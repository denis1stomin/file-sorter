using System.Collections.Generic;

namespace FileSorter.Common
{
    public class DataItemComparer : IComparer<DataItem>
    {
        public int Compare(DataItem d1, DataItem d2)
        {
            if (d1 == null)
                return -1;
            if (d2 == null)
                return 1;
            
            var numCompare = d1.Number.CompareTo(d2.Number);
            if (numCompare != 0)
                return numCompare;

            // TODO : use StringComparer to ignore some localization specifics.

            var textCompare = d1.Text.CompareTo(d2.Text);

            return textCompare;
        }
    }
}
