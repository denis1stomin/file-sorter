using System;
using System.IO;
using System.Text;

namespace FileSorter.Common
{
    public class FileDataWriter<T> : IDataWriter<T>, IDisposable
    {
        public Encoding Encoding { get; } = Encoding.UTF8;

        public FileDataWriter(string path, int bufSize)
            : this(Utils.CreateExclusiveWriteFile(path, bufSize))
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

        private readonly StreamWriter _writer;
    }
}
