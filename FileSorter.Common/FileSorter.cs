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
            PartitionInputData();
            MergeSortedPartitions();
        }

        private void EnsureTempFolderIsCreated()
        {
            if (!Directory.Exists(TempFolder))
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

        private readonly DataItemComparer Comparer = new DataItemComparer();
        private readonly Func<string, DataItem> Parser = x => new DataItem(x);
    }
}
