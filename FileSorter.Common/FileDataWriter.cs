using System;
using System.IO;
using System.Text;

namespace FileSorter.Common
{
    public class FileDataWriter<T> : IDataWriter<T>, IDisposable
    {
        public Encoding Encoding { get; } = Encoding.UTF8;

        public FileDataWriter(string path)
            : this(CreateExclusiveWriteFile(path))
        {
        }

        public FileDataWriter(FileStream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            // TODO : makes sense to provide int bufferSize to improve disk IO
            //        but the best size depends on available RAM and disk info etc.
            _writer = new StreamWriter(stream, Encoding);
        }

        public virtual void WriteItem(T item)
        {
            var str = item.ToString();
            this.WriteItem(str);
        }

        protected virtual void WriteItem(string item)
        {
            _writer.WriteLine(item);
        }

        public void Dispose()
        {
            _writer.Dispose();
        }

        private static FileStream CreateExclusiveWriteFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException(nameof(path));
            
            return new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        }

        private readonly StreamWriter _writer;
    }
}
