using System;
using FileSorter.Common;

namespace FileGenerator
{
    class DataGenerator
    {
        public DataItem NewItem()
        {
            return new DataItem(
                LongRandom(),
                "Some text here"
            );
        }

        private long LongRandom()
        {
            var buf = new byte[6];
            _rand.NextBytes(buf);

            Array.Resize(ref buf, 8);
            var longRand = BitConverter.ToInt64(buf, 0);

            return longRand;
        }

        private Random _rand = new Random();
    }
}
