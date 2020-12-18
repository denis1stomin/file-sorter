using System;
using System.Threading;
using System.Collections.Generic;

namespace FileSorter.Common
{
    public class PartitionMap : IPartitionMap, IDisposable
    {
        public PartitionMap()
        {
            _partitions = new SortedSet<PartitionInfo>(_partitionsComparer);
        }

        public bool WaitNewPartition(TimeSpan timeout)
        {
            return _newPartitionEvent.WaitOne(timeout);
        }

        public bool TakePartitionsToMerge(out PartitionInfo part1, out PartitionInfo part2)
        {
            lock (_locker)
            {
                if (_partitions.Count >= 2)
                {
                    part1 = PopMinUnsafe();
                    part2 = PopMinUnsafe();

                    return true;
                }
            }

            part1 = null;
            part2 = null;

            return false;
        }

        public void AddNewPartition(PartitionInfo partition)
        {
            Console.WriteLine($"Adding new partition '{partition.Partition.Name}/{partition.LinesCount}'");

            lock (_locker)
            {
                if (lastWasTaken)
                    throw new Exception("Something wrong here!");

                _partitions.Add(partition);
            }

            _newPartitionEvent.Set();
        }

        public PartitionInfo TakeLastPartition()
        {
            lock (_locker)
            {
                var last = PopMinUnsafe();
                if (_partitions.Count > 0)
                    throw new Exception("Something wrong here!");
                
                lastWasTaken = true;
                
                return last;
            }
        }

        public void Dispose()
        {
            _newPartitionEvent?.Dispose();
        }

        private PartitionInfo PopMinUnsafe()
        {
            var min = _partitions.Min;
            if (min != null)
            {
                bool done = _partitions.Remove(min);
                if (!done)
                    throw new Exception("Something wrong here!");
            }

            return min;
        }

        private readonly PartitionInfoComparer _partitionsComparer = new PartitionInfoComparer();
        private readonly SortedSet<PartitionInfo> _partitions;
        private readonly AutoResetEvent _newPartitionEvent = new AutoResetEvent(false);
        private readonly object _locker = new object();
        private bool lastWasTaken = false;

        private class PartitionInfoComparer : IComparer<PartitionInfo>
        {
            public int Compare(PartitionInfo p1, PartitionInfo p2)
            {
                if (p1.Partition.FullName.Equals(p2.Partition.FullName))
                    return 0;

                var compare = Math.Sign(p1.LinesCount - p2.LinesCount);

                // Helps to avoid deduplication limitation of SortedSet
                if (compare == 0)
                    return -1;

                return compare;
            }
        }
    }
}
