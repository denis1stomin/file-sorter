namespace FileSorter.Common
{
    public class SizedFileDataWriter<T> : FileDataWriter<T>
    {
        public long DesiredSize { get; }
        
        public SizedFileDataWriter(string path, long desiredSize, int bufSize)
            : base(path, bufSize)
        {
            DesiredSize = desiredSize;
        }

        public override void WriteItem(T item)
        {
            var str = item.ToString();
            var itemSize = this.Encoding.GetByteCount(str);

            // TODO : hm looks like this place ignores Liskov substitution principle.

            // ignore next data
            if (_currentSize > DesiredSize)
                return;

            base.WriteItem(str);
            _currentSize += itemSize;
        }

        public bool EnoughData()
        {
            // TODO : in practice result file can be little bit bigger than DesiredSize.
            //        Makes sense to crop the last input item to fit the desired size exactly.

            return _currentSize > DesiredSize;
        }

        private long _currentSize = 0;
    }
}
