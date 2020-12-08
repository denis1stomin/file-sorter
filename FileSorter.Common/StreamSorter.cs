using System;
using System.Collections.Generic;

namespace FileSorter.Common
{
    public class StreamSorter<T> : IStreamSorter<T> where T : IComparable
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
            : this(output, null, 10000)   // TODO : tune maxCount
        {
        }

        public StreamSorter(IDataWriter<T> output, IComparer<T> comparer, int maxCount)
        {
            if (comparer == null)
                _data = new SortedSet<T>();
            else
                _data = new SortedSet<T>(comparer);

            _output = output ?? throw new ArgumentNullException(nameof(output));
            MaxCount = (maxCount > 0) ? maxCount : throw new ArgumentException(nameof(maxCount));
        }

        public bool Add(T item)
        {
            // TODO : may be throw in the case some data added after Save() call.
            
            if (_data.Count == MaxCount)
                return false;

            _data.Add(item);
            return true;
        }

        public void Save()
        {
            foreach (var item in _data)
                _output.WriteItem(item);

            _data.Clear();
        }

        private readonly IDataWriter<T> _output;

        // Looks like a binary tree
        // https://github.com/microsoft/referencesource/blob/master/System/compmod/system/collections/generic/sortedset.cs
        private readonly SortedSet<T> _data;
    }
}
