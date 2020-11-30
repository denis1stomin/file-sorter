namespace FileSorter.Common
{
    public class SizedFileDataWriter : FileDataWriter
    {
        public long DesiredSize { get; }
        
        public SizedFileDataWriter(string path, long desiredSize)
            : base(path)
        {
            DesiredSize = desiredSize;
        }

        public override void WriteItem(DataItem item)
        {
            var str = item.ToString();
            var itemSize = this.Encoding.GetByteCount(str);
            // TODO : may be crop the item size

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
