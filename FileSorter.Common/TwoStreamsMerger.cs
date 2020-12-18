using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace FileSorter.Common
{
    public class TwoStreamsMerger<T> : IDisposable where T : class
    {
        public Encoding Encoding { get; } = Encoding.UTF8;

        public TwoStreamsMerger(
            Stream stream1, Stream stream2, Stream outputStream, IComparer<T> comparer, Func<string, T> parser)
        {
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));

            _stream1 =  (stream1 != null) ?
                new StreamReader(stream1, Encoding)
                    : throw new ArgumentNullException(nameof(stream1));

            _stream2 =  (stream2 != null) ?
                new StreamReader(stream2, Encoding)
                    : throw new ArgumentNullException(nameof(stream2));
            
            _outputStream =  (outputStream != null) ?
                new StreamWriter(outputStream, Encoding)
                    : throw new ArgumentNullException(nameof(outputStream));
        }

        public void Merge()
        {
            var item1 = ReadItem(_stream1);
            var item2 = ReadItem(_stream2);

            while ((item1 != null) && (item2 != null))
            {
                if (_comparer.Compare(item1, item2) < 0)
                    item1 = WriteItemAndGetNext(item1, _stream1);
                else
                    item2 = WriteItemAndGetNext(item2, _stream2);
            }

            // one of those (item1 or item2) equals to null at this point

            while (item1 != null)
                item1 = WriteItemAndGetNext(item1, _stream1);
            
            while (item2 != null)
                item2 = WriteItemAndGetNext(item2, _stream2);
        }

        public void Dispose()
        {
            _stream1?.Dispose();
            _stream2?.Dispose();
            _outputStream?.Dispose();
        }

        private T ReadItem(StreamReader stream)
        {
            var str = stream.ReadLine();

            T item = (str != null) ? _parser(str) : null;
            
            return item;
        }

        private T WriteItemAndGetNext(T item, StreamReader stream)
        {
            _outputStream.WriteLine(item.ToString());
            return ReadItem(stream);
        }

        private IComparer<T> _comparer;
        Func<string, T> _parser;

        private StreamReader _stream1;
        private StreamReader _stream2;
        private StreamWriter _outputStream;
    }
}
