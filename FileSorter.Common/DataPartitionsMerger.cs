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
            string partitionFolder, string destPath, int threadsNum, IComparer<T> dataComparer, Func<string, T> parser)
        {
            PartitionFolder = !string.IsNullOrWhiteSpace(partitionFolder) ? new DirectoryInfo(partitionFolder)
                : throw new ArgumentException(nameof(partitionFolder));

            DestinationPath = !string.IsNullOrWhiteSpace(destPath) ? destPath
                : throw new ArgumentException(nameof(destPath));
            
            _dataComparer = dataComparer ?? throw new ArgumentNullException(nameof(dataComparer));

            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        }

        public void StartWork()
        {
            var initialPartitions = PartitionFolder.EnumerateFiles("*.part").ToList();
            initialPartitions.Sort(new PartitionsComparer());

            var resultFile = MergePartitions(initialPartitions.Select(f => f.FullName).ToList());

            var p1 = DateTime.UtcNow;
            File.Delete(DestinationPath);
            File.Move(resultFile, DestinationPath);
            var p2 = DateTime.UtcNow;

            Console.WriteLine($"Just moving to destination took '{p2.Subtract(p1)}'");
        }

        private string MergePartitions(IEnumerable<string> partitions)
        {
            var cnt = partitions.Count();

            if (cnt == 1)
                return partitions.Single();
            
            if (cnt == 2)
            {
                var firstPath = partitions.First();
                var stream1 = Utils.OpenSharedReadFile(firstPath);

                var secondPath = partitions.Skip(1).First();
                var stream2 = Utils.OpenSharedReadFile(secondPath);

                var mergedPath = $"{PartitionFolder}/{Guid.NewGuid()}.part";
                var outputStream = Utils.CreateExclusiveWriteFile(mergedPath);

                using (var pairMerger = new TwoStreamsMerger<T>(
                    stream1, stream2, outputStream, _dataComparer, _parser))
                {
                    pairMerger.Merge();
                }

                File.Delete(firstPath);
                File.Delete(secondPath);

                return mergedPath;
            }

            // simple split
            int splitSize = partitions.Count() / 2;
            string newPart1 = MergePartitions(partitions.Take(splitSize).ToList());
            string newPart2 = MergePartitions(partitions.Skip(splitSize).ToList());

            return MergePartitions(new List<string>() {
                newPart1, newPart2
            });
        }

        private IComparer<T> _dataComparer;
        private Func<string, T> _parser;

        private class PartitionsComparer : IComparer<FileInfo>
        {
            public int Compare(FileInfo p1, FileInfo p2)
            {
                var linesCnt1 = int.Parse(p1.Name.Split('_').First());
                var linesCnt2 = int.Parse(p2.Name.Split('_').First());

                return linesCnt1.CompareTo(linesCnt2);
            }
        }
    }
}
