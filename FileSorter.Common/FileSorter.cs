using System;
using System.IO;

namespace FileSorter.Common
{
    public class FileSorter
    {
        public string SourcePath { get; }
        public string DestPath { get; }
        public string TempFolder { get; }
        public long TempFileMaxSize { get; }

        public FileSorter(string sourcePath, string destPath, string tempDir, long tempSize)
        {
            SourcePath = !string.IsNullOrWhiteSpace(sourcePath) ?
                sourcePath : throw new ArgumentException(nameof(sourcePath));
            
            DestPath = !string.IsNullOrWhiteSpace(destPath) ?
                destPath : throw new ArgumentException(nameof(destPath));
            
            TempFolder = !string.IsNullOrWhiteSpace(tempDir) ?
                tempDir : throw new ArgumentException(nameof(tempDir));

            TempFileMaxSize = (tempSize > 0) ? tempSize
                : throw new ArgumentException($"{nameof(tempSize)} parameter is '{tempSize}'");
        }

        public void Sort()
        {
            EnsureTempFolderIsCreated();

            var p1 = DateTime.UtcNow;
            PartitionInputData();
            var p2 = DateTime.UtcNow;
            MergeSortedPartitions();
            var p3 = DateTime.UtcNow;

            Console.WriteLine($"Partitioning step took '{p2.Subtract(p1)}'");
            Console.WriteLine($"Merging step took '{p3.Subtract(p2)}'");
        }

        private void EnsureTempFolderIsCreated()
        {
            if (Directory.Exists(TempFolder))
                Directory.Delete(TempFolder);
            
            Directory.CreateDirectory(TempFolder);
        }

        private void PartitionInputData()
        {
            using (var sourceReader = new FileDataReader<DataItem>(SourcePath, Parser))
            {
                var partitioner = new DataPartitionerSorter<DataItem>(
                    sourceReader, TempFolder, TempFileMaxSize, Comparer);

                partitioner.StartWork();
            }
        }

        private void MergeSortedPartitions()
        {
            var merger = new DataPartitionsMerger<DataItem>(TempFolder, DestPath, 1, Comparer, Parser);
            merger.StartWork();
        }

        private readonly DataItemTrickyComparer Comparer = new DataItemTrickyComparer();
        private readonly Func<string, DataItem> Parser = x => new DataItem(x);
    }
}
