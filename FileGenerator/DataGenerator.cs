using System;
using System.Collections.Generic;
using FileSorter.Common;

namespace FileGenerator
{
    public class DataGenerator
    {
        public const int RepeatPercentage = 20;
        public const int MaxMemorySize = 10000;

        public DataGenerator()
        {
            if (RepeatPercentage < 1 || RepeatPercentage > 49)
                throw new ArgumentException($"{nameof(RepeatPercentage)} cannot be lower than 1 and greater than 49.");
            
            _longSubBytes = new ArraySegment<byte>(_longBytes, 0, 7);   // almost all positive int64 values
        }

        public DataItem NewItem()
        {
            _itemIndex ++;

            DataItem item = null;

            // get each RNDth from previously remembered
            if ((_itemIndex % _fromMemoryIdx == 0) && _somePrevItems.Count > 0)
            {
                item = _somePrevItems[_rand.Next(_somePrevItems.Count)];
            }
            else
            {
                item = new DataItem(
                    RandomLong2(),
                    RandomString(_rand.Next(MaxTextLength))
                );
            }

            // remember each previous before repeat
            if ((_somePrevItems.Count < MaxMemorySize) && (_itemIndex % (_fromMemoryIdx - 1) == 0))
                _somePrevItems.Add(item);
            
            return item;
        }

        private string RandomString(int size)
        {
            Span<char> textChars = stackalloc char[MaxTextLength];

            for (int i = 0; i < size; i++)
                textChars[i] = PossibleChars[_rand.Next(PossibleCharsLength)];

            return new String(textChars.ToArray(), 0, size);
        }

        private long RandomLong()
        {
            _rand.NextBytes(_longSubBytes);
            var longRand = BitConverter.ToInt64(_longBytes);

            return longRand;
        }

        private long RandomLong2()
        {
            long res = _rand.Next();

            //shift the bits creating an empty space on the right
            // ex: 0x0000CFFF becomes 0xCFFF0000
            res = (res << 32);

            //combine the bits on the right with the previous value
            // ex: 0xCFFF0000 | 0x0000ABCD becomes 0xCFFFABCD
            res = res | (long)/*(uint)*/ _rand.Next(); //uint first to prevent loss of signed bit

            return res;
        }

        private long _itemIndex = 0;

        private readonly char[] PossibleChars = " -ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789АБВГДЕЁЖЗИКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзиклмнопрстуфхцчшщъыьэюя".ToCharArray();
        private const int PossibleCharsLength = 128;
        private readonly Random _rand = new Random();
        private readonly List<DataItem> _somePrevItems = new List<DataItem>();
        private readonly byte[] _longBytes = new byte[8];
        private readonly ArraySegment<byte> _longSubBytes;
        private const int MaxTextLength = 50;

        private const int _fromMemoryIdx = 100 / RepeatPercentage;
    }
}
