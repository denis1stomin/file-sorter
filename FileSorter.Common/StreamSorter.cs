using System;
using System.Collections.Generic;

namespace FileSorter.Common
{
    public class StreamSorter<T> : IStreamSorter<T>
    {
        public int Count
        {
            get
            {
                return _data.Count;
            }
        }

        public int MaxCount { get; }

        public StreamSorter(IDataWriter<T> output)
            : this(output, 10000)   // TODO : tune maxCount
        {
        }

        public StreamSorter(IDataWriter<T> output,int maxCount)
        {
            _output = output ?? throw new ArgumentNullException(nameof(output));
            MaxCount = (maxCount > 0) ? maxCount : throw new ArgumentException(nameof(maxCount));;
        }

        public bool Add(T item)
        {
            if (_data.Count == MaxCount)
                return false;

            _data.Add(item);
            return true;
        }

        public void Save()
        {
            foreach (var item in _data)
                _output.WriteItem(item);
        }

        private IDataWriter<T> _output;

        // Should be a binary tree
        // https://github.com/microsoft/referencesource/blob/master/System/compmod/system/collections/generic/sortedset.cs
        private SortedSet<T> _data = new SortedSet<T>();
    }
}
