using System;
using System.IO;
using System.Collections.Generic;
using FileSorter.Common;

namespace FileGenerator
{
    public class DataGenerator
    {
        public const int RepeatPercentage = 20;

        public DataGenerator()
        {
            if (RepeatPercentage < 1 || RepeatPercentage > 49)
                throw new ArgumentException($"{nameof(RepeatPercentage)} cannot be lower than 1 and greater than 49.");
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
                    RandomLong(),
                    RandomString(_rand.Next(MaxTextLength))
                );
            }

            // remember each previous before repeat
            if (_itemIndex % (_fromMemoryIdx - 1) == 0)
                _somePrevItems.Add(item);
            
            return item;
        }

        private string RandomString(int size)
        {
            var maxIdx = PossibleChars.Length;

            for (int i = 0; i < size; i++)
                _stringChars[i] = PossibleChars[_rand.Next(maxIdx)];

            return new String(_stringChars, 0, size);
        }

        private long RandomLong()
        {
            _rand.NextBytes(_longBytes);
            var longRand = BitConverter.ToInt64(_longBytes);

            return longRand;
        }

        private char[] PossibleChars = " -ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789АБВГДЕЁЖЗИКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзиклмнопрстуфхцчшщъыьэюя".ToCharArray();
        private const int MaxTextLength = 50;
        private Random _rand = new Random();
        private List<DataItem> _somePrevItems = new List<DataItem>();
        private long _itemIndex = 0;
        private byte[] _longBytes = new byte[8];
        private char[] _stringChars = new char[MaxTextLength];

        private const int _fromMemoryIdx = 100 / RepeatPercentage;  // brutal presicion)
    }
}
