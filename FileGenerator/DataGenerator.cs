using System;
using System.IO;
using System.Collections.Generic;
using FileSorter.Common;

namespace FileGenerator
{
    public class DataGenerator
    {
        public DataItem NewItem()
        {
            DataItem item = null;

            // TODO : make 30 and 40 - random values too.

            // get each RNDth from previously remembered
            //   (1/40 % of repeats)
            if ((_itemIndex % 40 == 0) && _somePrevItems.Count > 0)
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

            // remember each 30th item
            if (_itemIndex % 30 == 0)
                _somePrevItems.Add(item);
            
            _itemIndex ++;

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

        private const string PossibleChars = " -ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private const int MaxTextLength = 50;
        private Random _rand = new Random();
        private List<DataItem> _somePrevItems = new List<DataItem>();
        private long _itemIndex = 0;
        private byte[] _longBytes = new byte[8];
        private char[] _stringChars = new char[MaxTextLength];
    }
}
