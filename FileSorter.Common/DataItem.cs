using System;

namespace FileSorter.Common
{
    public class DataItem : IComparable<DataItem>
    {
        public long Number;

        public string Text
        {
            get
            {
                if (_text == null)
                {
                    if (_serialized.Length > _separatorIdx)
                        _text = _serialized.Substring(_separatorIdx + 1);
                    else
                        _text = string.Empty;
                }
                
                return _text;
            }
        }

        public DataItem(string serializedObj)
        {
            _serialized = serializedObj;

            var separator = '.';
            _separatorIdx = _serialized.IndexOf(separator);
            if (_separatorIdx < 0)
                throw new FormatException($"Cannot find separator symbol '{separator}' in data line '{_serialized}'.");

            var strNumber = _serialized.Substring(0, _separatorIdx);

            if (!long.TryParse(strNumber, out Number))
                throw new FormatException($"Cannot convert '{strNumber}' to long type.");
        }

        public DataItem(long number, string text)
        {
            Number = number;
            _text = text ?? throw new ArgumentException(nameof(text));
        }

        public override string ToString()
        {
            if (_serialized == null)
                _serialized = $"{Number}. {Text}";

            return _serialized;
        }

        public int CompareTo(DataItem other)
        {
            if (other == null)
                return 1;

            var numCompare = this.Number.CompareTo(other.Number);
            if (numCompare != 0)
                return numCompare;

            // TODO : use StringComparer to ignore some localization specifics.

            var textCompare = this.Text.CompareTo(other.Text);

            return textCompare;
        }

        private int _separatorIdx = -1;
        private string _text;
        private string _serialized;
    }
}
