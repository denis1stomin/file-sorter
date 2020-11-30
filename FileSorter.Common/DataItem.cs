using System;

namespace FileSorter.Common
{
    public class DataItem
    {
        public long Number { get; }
        public string Text { get; }

        public DataItem(long number, string text)
        {
            Number = number;
            Text = text ?? throw new ArgumentException(nameof(text));
        }

        public override string ToString()
        {
            return $"{Number}. {Text}";
        }
    }
}
