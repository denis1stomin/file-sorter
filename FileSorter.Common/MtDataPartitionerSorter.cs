using System;
using System.Threading;
using System.Collections.Generic;

namespace FileSorter.Common
{
    public class MtDataPartitionerSorter<T> : DataPartitionerSorter<T> where T : class
    {
        public int ThreadsMax { get; } = Environment.ProcessorCount;

        public MtDataPartitionerSorter(
            IDataReader<T> dataReader, string partitionFolder, long partitionMaxSize,
            IComparer<T> dataComparer, int threadsMax, PartitionMap partitionMap)

            : base(dataReader, partitionFolder, partitionMaxSize, dataComparer)
        {
            ThreadsMax = (threadsMax > 0) ? threadsMax : throw new ArgumentException(nameof(threadsMax));
            _partitionMap = partitionMap ?? throw new ArgumentNullException(nameof(partitionMap));
        }

        public override void StartWork(bool wait = true)
        {
            _readerThread = new Thread(new ThreadStart(ReaderThreadFunc));
            _readerThread.Start();

            if (wait)
                WaitWorkFinished();
        }

        public override void WaitWorkFinished()
        {
            _readerThread.Join();
            
            while (_workerThreads.Count > 0)
                Thread.Sleep(50);
        }

        private void ReaderThreadFunc()
        {
            try
            {
                var partition = new List<T>((int)PartitionMaxSize);

                var next = _dataReader.NextItem();
                while (next != null)
                {
                    while (_workerThreads.Count >= ThreadsMax)
                        Thread.Sleep(50);

                    partition.Add(next);

                    if (partition.Count == PartitionMaxSize)
                    {
                        InvokeWorkerThreadForPartition(partition);
                        partition = new List<T>((int)PartitionMaxSize);
                    }

                    next = _dataReader.NextItem();
                }

                if (partition.Count > 0)
                    InvokeWorkerThreadForPartition(partition);

                partition = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Got an error {ex}, {ex.Message}");
                throw;
            }
        }

        private void InvokeWorkerThreadForPartition(List<T> partition)
        {
            var t = new Thread(new ParameterizedThreadStart(WorkerThreadFunc));

            lock (_workerThreads)
                _workerThreads.Add(t);

            t.Start(partition);
        }

        private void WorkerThreadFunc(object partition)
        {
            WorkerThreadFunc(
                partition as List<T> ?? throw new ArgumentException(nameof(partition))
            );
        }

        private void WorkerThreadFunc(List<T> partition)
        {
            try
            {
                partition.Sort(_dataComparer);
                var linesCount = partition.Count;

                var fileInfo = SavePartition(partition, PartitionFolder);
                
                _partitionMap.AddNewPartition(
                    new PartitionInfo(fileInfo, linesCount));

                lock (_workerThreads)
                    _workerThreads.Remove(Thread.CurrentThread);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Got an error {ex}, {ex.Message}");
                throw;
            }
        }

        private Thread _readerThread;
        private readonly List<Thread> _workerThreads = new List<Thread>();
        private readonly PartitionMap _partitionMap = new PartitionMap();
    }
}
