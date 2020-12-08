using System;
using System.IO;
using System.Text;

namespace FileSorter.Common
{
    public class FileDataReader<T> : IDataReader<T> where T : class
    {
        public Encoding Encoding { get; } = Encoding.UTF8;

        public FileDataReader(string filePath, Func<string, T> parser)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException(nameof(filePath));
            
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));

            var stream = Utils.OpenSharedReadFile(filePath);
            _reader = new StreamReader(stream, Encoding);
        }

        public T NextItem()
        {
            var str = _reader.ReadLine();

            if (str == null)
                return null;

            return _parser(str);
        }

        public void Dispose()
        {
            _reader?.Dispose();
        }

        private Func<string, T> _parser;
        private TextReader _reader;
    }
}
