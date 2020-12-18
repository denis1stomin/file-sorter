﻿using System;
using System.IO;

namespace FileSorter.Common
{
    public class FileSorter : IDisposable
    {
        public string SourcePath { get; }
        public string DestPath { get; }
        public string TempFolder { get; }
        public long TempFileMaxSize { get; }
        public int PartitionerThreadsNum { get; } = Environment.ProcessorCount / 2;
        public int MergerThreadsNum { get; } = 2;

        public FileSorter(string sourcePath, string destPath, string tempDir, long tempSize, int threadsNum)
        {
            SourcePath = !string.IsNullOrWhiteSpace(sourcePath) ?
                sourcePath : throw new ArgumentException(nameof(sourcePath));
            
            DestPath = !string.IsNullOrWhiteSpace(destPath) ?
                destPath : throw new ArgumentException(nameof(destPath));
            
            TempFolder = !string.IsNullOrWhiteSpace(tempDir) ?
                tempDir : throw new ArgumentException(nameof(tempDir));

            TempFileMaxSize = (tempSize > 0) ? tempSize
                : throw new ArgumentException($"{nameof(tempSize)} parameter is '{tempSize}'");
            
            PartitionerThreadsNum = (threadsNum > 0 && threadsNum < 30) ? threadsNum
                : throw new ArgumentException($"{nameof(threadsNum)} parameter is '{threadsNum}'");
        }

        public void Sort()
        {
            EnsureTempFolderIsCreated();

            var p1 = DateTime.UtcNow;
            var partitioner = StartPartitioningInputData();
            var merger = StartMergingSortedPartitions();

            partitioner.WaitWorkFinished();
            partitioner = null;
            GC.Collect();
            var p2 = DateTime.UtcNow;

            merger.SignalNoMoreNewPartitions();
            merger.StartMoreWorkers(MergerThreadsNum);
            merger.WaitWorkFinished();
            var p3 = DateTime.UtcNow;

            Console.WriteLine($"Partitioning step took '{p2.Subtract(p1)}'");
            Console.WriteLine($"Merging step finish part took '{p3.Subtract(p2)}'");
        }

        public void Dispose()
        {
            _sourceReader?.Dispose();
        }

        private void EnsureTempFolderIsCreated()
        {
            if (Directory.Exists(TempFolder))
                Directory.Delete(TempFolder, true);
            
            Directory.CreateDirectory(TempFolder);
        }

        private IDataPartitionerSorter<DataItem> StartPartitioningInputData()
        {
            _sourceReader = new FileDataReader<DataItem>(SourcePath, _parser);

#warning  DataItemComparer(ignoreText = true) if partition size is bigger than whole source file the partition will be sorted with no respect to text field.

            var partitioner = new MtDataPartitionerSorter<DataItem>(
                _sourceReader, TempFolder, TempFileMaxSize, new DataItemComparer(true), PartitionerThreadsNum, _partitionMap);

            partitioner.StartWork(false);

            return partitioner;
        }

        private IDataPartitionsMerger<DataItem> StartMergingSortedPartitions()
        {
            var merger = new MtDataPartitionsMerger<DataItem>(
                TempFolder, DestPath, new DataItemComparer(false), _parser, MergerThreadsNum, _partitionMap);

            merger.StartWork();

            return merger;
        }

        FileDataReader<DataItem> _sourceReader;
        private readonly Func<string, DataItem> _parser = x => new DataItem(x);
        private readonly PartitionMap _partitionMap = new PartitionMap();
    }
}
