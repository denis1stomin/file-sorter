using System;
using System.IO;

namespace FileSorter.Common
{
    public class FileSorter
    {
        public string SourcePath { get; }
        public string DestPath { get; }
        public string TempFolder { get; } = "./.srt_tmp";
        public long TempFileMaxSize { get; } = 10;

        public FileSorter(string sourcePath, string destPath)
        {
            SourcePath = !string.IsNullOrWhiteSpace(sourcePath) ?
                sourcePath : throw new ArgumentException(nameof(sourcePath));
            
            DestPath = !string.IsNullOrWhiteSpace(destPath) ?
                destPath : throw new ArgumentException(nameof(destPath));
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
            var portionPath = $"{TempFolder}/tmp.1.txt";
            using (var sourceReader = new FileDataReader<DataItem>(SourcePath, s => new DataItem(s)))
            {
                var partitioner = new DataPartitionerSorter<DataItem>(
                    sourceReader, TempFolder, TempFileMaxSize, new DataItemComparer());

                partitioner.StartWork();
            }
        }

        private void MergeSortedPartitions()
        {

        }
    }
}
