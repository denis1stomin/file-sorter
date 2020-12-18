using System;
using System.Collections.Generic;

namespace FileSorter.Common
{
    public class DataItemComparer : IComparer<DataItem>
    {
        public DataItemComparer(bool ignoreText = false)
        {
            _ignoreText = ignoreText;
        }

        public int Compare(DataItem d1, DataItem d2)
        {
            if (d1 == null)
                return -1;
            if (d2 == null)
                return 1;
            
            var numCompare = d1.Number.CompareTo(d2.Number);
            if (numCompare != 0)
                return numCompare;

            if (!_ignoreText)
                return _textComparer.Compare(d1.Text, d2.Text);

            return 0;
        }

        private bool _ignoreText = false;
        private StringComparer _textComparer = StringComparer.Ordinal;      // ignore culture
    }
}
