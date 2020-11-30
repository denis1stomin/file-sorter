using System;
using System.IO;
using System.Text;

namespace FileSorter.Common
{
    public class FileDataWriter : IDataWriter, IDisposable
    {
        public Encoding Encoding { get; } = Encoding.UTF8;

        public FileDataWriter(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException(nameof(path));

            var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            _writer = new StreamWriter(stream, Encoding);
        }

        public virtual void WriteItem(DataItem item)
        {
            var str = item.ToString();
            _writer.WriteLine(str);
        }

        public virtual void WriteItem(string item)
        {
            _writer.WriteLine(item);
        }

        public void Dispose()
        {
            _writer.Dispose();
        }

        private readonly StreamWriter _writer;
    }
}
