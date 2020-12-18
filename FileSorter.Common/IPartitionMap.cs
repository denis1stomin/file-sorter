using System;
using System.IO;

namespace FileSorter.Common
{
    public class PartitionInfo
    {
        public FileInfo Partition { get; }
        public long LinesCount { get; }

        public PartitionInfo(FileInfo fileInfo,long count)
        {
            Partition = fileInfo;
            LinesCount = count;
        }
    }

    public interface IPartitionMap
    {
        /// Blocks calling thread till new partition arrives or time out.
        bool WaitNewPartition(TimeSpan timeout);

        /// Adds a new partition into internal queue.
        /// Waits merge ticket if partition is a result of merge.
        void AddNewPartition(PartitionInfo partition, Guid? mergeTicket = null);

        /// Returns two partitions for merge and merge ticket (kinda merge operation cookie).
        bool TakePartitionsForMerge(out PartitionInfo part1, out PartitionInfo part2, out Guid mergeTicket);

        /// Returns the number of in-progress merges (active merge tickets).
        int GetInProgressMergeCount();

        /// Returns a very last single partition.
        PartitionInfo TakeLastPartition();
    }
}
