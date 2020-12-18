using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace FileSorter.Common
{
    public class MtDataPartitionsMerger<T> : DataPartitionsMerger<T>, IDisposable where T : class
    {
        public int ThreadsMax { get; } = Environment.ProcessorCount;

        public MtDataPartitionsMerger(
            string partitionFolder, string destPath, IComparer<T> dataComparer,
            Func<string, T> parser, int threadsMax, PartitionMap partitionMap)

            : base(partitionFolder, destPath, dataComparer, parser)
        {
            ThreadsMax = (threadsMax > 0) ? threadsMax : throw new ArgumentException(nameof(threadsMax));
            _partitionMap = partitionMap ?? throw new ArgumentNullException(nameof(partitionMap));
        }

        public override void StartWork()
        {
            for (int i = ThreadsMax; i > 0; --i)
            {
                var t = new Thread(new ThreadStart(ThreadFunc));
                _threads.Add(t);
                t.Start();
            }
        }

        public override void WaitWorkFinished()
        {
            Console.WriteLine($"Waiting merger threads are finished.");
            foreach (var t in _threads)
                t.Join();
        }

        public override void SignalNoMoreNewPartitions()
        {
            Console.WriteLine($"Signalling no more new partitions.");
            _noMorePartitionsEvent.Set();
        }

        public void Dispose()
        {
            _noMorePartitionsEvent?.Dispose();
        }

        private void ThreadFunc()
        {
            try
            {
                PartitionInfo part1 = null;
                PartitionInfo part2 = null;

                while (_partitionMap.TakePartitionsToMerge(out part1, out part2) || NewPartitionsArePossible)
                {
                    if (part1 != null && part2 != null)
                    {
                        var newPart = MergeTwoPartitions(part1, part2);
                        _partitionMap.AddNewPartition(newPart);

                        part1 = null;
                        part2 = null;
                    }
                    else
                    {
                        Console.Write($"Waiting new partitions...");

                        _partitionMap.WaitNewPartition(TimeSpan.FromSeconds(1));
                    }
                }

                var lastPart = _partitionMap.TakeLastPartition();
                if (lastPart != null)
                {
                    var p1 = DateTime.UtcNow;
                    if (File.Exists(DestinationPath))
                        File.Delete(DestinationPath);

                    File.Move(lastPart.Partition.FullName, DestinationPath);
                    var p2 = DateTime.UtcNow;

                    Console.WriteLine($"Just moving to destination took '{p2.Subtract(p1)}'");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Got an error {ex}, {ex.Message}");
                throw;
            }
        }

        private int WorkerThreadsCount
        {
            get
            {
                lock (_threads)
                    return _threads.Count;
            }
        }

        private bool NewPartitionsArePossible
        {
            get
            {
                var isSet = _noMorePartitionsEvent.WaitOne(0);
                return !isSet;
            }
        }

        private readonly List<Thread> _threads = new List<Thread>();
        private readonly PartitionMap _partitionMap = new PartitionMap();
        private readonly int _currentlyInProgressWorkersCount = 0;
        private readonly ManualResetEvent _noMorePartitionsEvent = new ManualResetEvent(false);

        private class MergerThreadArg
        {
            public PartitionInfo Partition1 { get; set; }
            public PartitionInfo Partition2 { get; set; }
        }
    }
}
