using System;
using System.IO;

namespace FileSorter.Common
{
    public class DataFileReader : IDataReader
    {
        public DataFileReader(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException(nameof(filePath));
            

        }

        public DataItem NextItem()
        {
            return new DataItem(123, "some text here");
        }

        private TextReader _reader;
    }
}
