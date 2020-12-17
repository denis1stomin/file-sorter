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
        bool WaitNewPartition(TimeSpan timeout);

        void AddNewPartition(PartitionInfo partition);

        bool TakePartitionsToMerge(out PartitionInfo part1, out PartitionInfo part2);

        PartitionInfo TakeLastPartition();
    }
}
