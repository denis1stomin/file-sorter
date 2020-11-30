using System;
using System.IO;
using System.Collections.Generic;
using FileSorter.Common;

namespace FileGenerator
{
    class DataGenerator
    {
        public DataItem NewItem()
        {
            DataItem item = null;

            // get each 40th from previously remembered
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
        private List<DataItem> _somePrevItems = new List<DataItem>();
        private long _itemIndex = 0;
    }
}
