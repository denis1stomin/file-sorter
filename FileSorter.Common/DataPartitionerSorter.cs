using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace FileSorter.Common
{
    public class DataPartitionerSorter<T> : IDataPartitionerSorter<T>
    {
        public string PartitionFolder { get; }

        public long PartitionMaxSize { get; }

        // Just empirical coef for current type of data.
        // definitely should be changed by proper partition bytes calculating in the future.
        public int EmpiricalAverageItemSize { get; } = 59;

        public static Encoding Encoding { get; } = Encoding.UTF8;

        public static int FileBufferSize { get; } = 1024 * 1024 * 64;

        public DataPartitionerSorter(
            IDataReader<T> dataReader, string partitionFolder, long partitionMaxSize, IComparer<T> dataComparer)
        {
            _dataReader = dataReader ?? throw new ArgumentNullException(nameof(dataReader));

            PartitionFolder = !string.IsNullOrWhiteSpace(partitionFolder) ? partitionFolder
                : throw new ArgumentException(nameof(partitionFolder));
            
            PartitionMaxSize = (partitionMaxSize > 0) ? partitionMaxSize / EmpiricalAverageItemSize
                : throw new ArgumentException(nameof(partitionMaxSize));
            
            _dataComparer = dataComparer ?? throw new ArgumentNullException(nameof(dataComparer));
        }

        public virtual void StartWork(bool wait = true)
        {
            // Looks like a binary tree
            // https://github.com/microsoft/referencesource/blob/master/System/compmod/system/collections/generic/sortedset.cs
            var data = new SortedSet<T>(_dataComparer);

            var next = _dataReader.NextItem();
            while (next != null)
            {
                data.Add(next);

                // TODO : currently _data.Count is not bytes!!!
                if (data.Count >= PartitionMaxSize)
                    SavePartition(data, PartitionFolder);
                
                next = _dataReader.NextItem();
            }

            if (data.Count > 0)
                SavePartition(data, PartitionFolder);
        }

        public virtual void WaitWorkFinished()
        {
            throw new NotImplementedException();
        }

        protected static FileInfo SavePartition(ICollection<T> data, string saveFolder)
        {
            var partitionPath = $"{saveFolder}/{data.Count}_{Guid.NewGuid()}.part";
            var stream = Utils.CreateExclusiveWriteFile(partitionPath, FileBufferSize);

            using (var writer = new StreamWriter(stream, Encoding))
            {
                foreach (var el in data)
                    writer.WriteLine(el);

                writer.Close();
            }

            data.Clear();
            GC.Collect();

            return new FileInfo(partitionPath);
        }

        protected readonly IDataReader<T> _dataReader;
        protected readonly IComparer<T> _dataComparer;
    }
}
