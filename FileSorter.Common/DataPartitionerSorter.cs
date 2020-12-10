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

        public Encoding Encoding { get; } = Encoding.UTF8;

        public DataPartitionerSorter(
            IDataReader<T> dataReader, string partitionFolder, long partitionMaxSize, IComparer<T> dataComparer)
        {
            _dataReader = dataReader ?? throw new ArgumentNullException(nameof(dataReader));

            PartitionFolder = !string.IsNullOrWhiteSpace(partitionFolder) ? partitionFolder
                : throw new ArgumentException(nameof(partitionFolder));
            
            PartitionMaxSize = (partitionMaxSize > 0) ? partitionMaxSize
                : throw new ArgumentException(nameof(partitionMaxSize));

            _data = (dataComparer != null) ? new SortedSet<T>(dataComparer)
                : throw new ArgumentNullException(nameof(dataComparer));
        }

        public void StartWork()
        {
            // TODO : if disk IO is good it makes sense to parallel the loop below.

            var next = _dataReader.NextItem();
            while (next != null)
            {
                _data.Add(next);

                // TODO : currently _data.Count is not bytes!!!
                if (_data.Count >= PartitionMaxSize)
                    SavePartition();
                
                next = _dataReader.NextItem();
            }

            if (_data.Count > 0)
                SavePartition();
        }

        private void SavePartition()
        {
            var partitionPath = $"{PartitionFolder}/{_data.Count}_{_fileUniqueSuffix ++}.part";
            var stream = Utils.CreateExclusiveWriteFile(partitionPath);

            using (var writer = new StreamWriter(stream, Encoding))
            {
                foreach (var el in _data)
                    writer.WriteLine(el);

                writer.Close();
            }

            _data.Clear();
        }

        private int _fileUniqueSuffix = 1;
        private readonly IDataReader<T> _dataReader;

        // Looks like a binary tree
        // https://github.com/microsoft/referencesource/blob/master/System/compmod/system/collections/generic/sortedset.cs
        private readonly SortedSet<T> _data;
    }
}
