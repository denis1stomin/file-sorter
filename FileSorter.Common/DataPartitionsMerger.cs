using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace FileSorter.Common
{
    public class DataPartitionsMerger<T> : IDataPartitionsMerger<T> where T : class
    {
        public DirectoryInfo PartitionFolder { get; }

        public string DestinationPath { get; }

        public Encoding Encoding { get; } = Encoding.UTF8;

        public DataPartitionsMerger(
            string partitionFolder, string destPath, IComparer<T> dataComparer, Func<string, T> parser)
        {
            PartitionFolder = !string.IsNullOrWhiteSpace(partitionFolder) ? new DirectoryInfo(partitionFolder)
                : throw new ArgumentException(nameof(partitionFolder));

            DestinationPath = !string.IsNullOrWhiteSpace(destPath) ? destPath
                : throw new ArgumentException(nameof(destPath));
            
            _dataComparer = dataComparer ?? throw new ArgumentNullException(nameof(dataComparer));

            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        }

        public virtual void StartWork()
        {
            var initialPartitions = GetPartitionsInfo();
            initialPartitions.Sort((x, y) => {
                return (int)(y.LinesCount - x.LinesCount);
            });

            var resultFile = MergePartitions(initialPartitions);

            var p1 = DateTime.UtcNow;
            File.Delete(DestinationPath);
            File.Move(resultFile.Partition.FullName, DestinationPath);
            var p2 = DateTime.UtcNow;

            Console.WriteLine($"Just moving to destination took '{p2.Subtract(p1)}'");
        }

        public virtual void StartMoreWorkers(int num)
        {
            throw new NotImplementedException();
        }

        public virtual void FinishWork()
        {
            throw new NotImplementedException();
        }

        public virtual void SignalNoMoreNewPartitions()
        {
            throw new NotImplementedException();
        }

        protected List<PartitionInfo> GetPartitionsInfo()
        {
            var partitions = PartitionFolder.EnumerateFiles("*.part")
                .Select(f => {
                    var linesCnt = int.Parse(f.Name.Split('_').First());
                    return new PartitionInfo(f, linesCnt);
                }).ToList();
            
            return partitions;
        }

        protected PartitionInfo MergeTwoPartitions(PartitionInfo first, PartitionInfo second)
        {
            //Console.WriteLine($"Merging parts: '{first.Partition.Name}/{first.LinesCount}' and '{second.Partition.Name}/{second.LinesCount}'");

            var firstPath = first.Partition.FullName;
            var stream1 = Utils.OpenSharedReadFile(firstPath);

            var secondPath = second.Partition.FullName;
            var stream2 = Utils.OpenSharedReadFile(secondPath);

            var linesSum = first.LinesCount + second.LinesCount;

            var mergedPath = $"{PartitionFolder}/{linesSum}_{Guid.NewGuid()}.part";
            var outputStream = Utils.CreateExclusiveWriteFile(mergedPath, 1024 * 1024 * 32);

            using (var pairMerger = new TwoStreamsMerger<T>(
                stream1, stream2, outputStream, _dataComparer, _parser))
            {
                pairMerger.Merge();
            }

            File.Delete(firstPath);
            File.Delete(secondPath);

            return new PartitionInfo(new FileInfo(mergedPath), linesSum);
        }

        private PartitionInfo MergePartitions(IEnumerable<PartitionInfo> partitions)
        {
            var cnt = partitions.Count();

            if (cnt == 1)
                return partitions.Single();
            
            if (cnt == 2)
            {
                var firstPath = partitions.First();
                var secondPath = partitions.Skip(1).First();
                
                return MergeTwoPartitions(firstPath, secondPath);
            }

            // simple split
            int splitSize = partitions.Count() / 2;
            var newPart1 = MergePartitions(partitions.Take(splitSize).ToList());
            var newPart2 = MergePartitions(partitions.Skip(splitSize).ToList());

            return MergePartitions(new List<PartitionInfo>() {
                newPart1, newPart2
            });
        }

        protected IComparer<T> _dataComparer;
        protected Func<string, T> _parser;
    }
}
