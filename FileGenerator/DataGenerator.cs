using System;
using FileSorter.Common;

namespace FileGenerator
{
    class DataGenerator
    {
        public DataItem NewItem()
        {
            return new DataItem(
                RandomLong(),
                RandomString(_rand.Next(MaxTextLength))
            );
        }

        private string RandomString(int size)
        {
            var stringChars = new char[size];
            for (int i = 0; i < stringChars.Length; i++)
                stringChars[i] = PossibleChars[_rand.Next(PossibleChars.Length)];

            return new String(stringChars);
        }

        private long RandomLong()
        {
            var buf = new byte[6];
            _rand.NextBytes(buf);

            Array.Resize(ref buf, 8);
            var longRand = BitConverter.ToInt64(buf, 0);

            return longRand;
        }

        private const string PossibleChars = " -ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private const int MaxTextLength = 150;
        private Random _rand = new Random();
    }
}
