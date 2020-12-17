using System;
using System.Collections.Generic;
using FileSorter.Common;

namespace SortChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            var t1 = DateTime.UtcNow;

            var filePath = "sorted.txt";
            if (args.Length > 0)
                filePath = args[0];
            
            using (var reader = new FileDataReader<DataItem>(filePath, s => new DataItem(s)))
            {
                var comparer = new DataItemComparer();

                long overallRepeatCnt = 0;
                long itemsCnt = 1;
                long seqRepeatCnt = 0;
                long maxSeqRepeatCnt = 0;
                long sortFaults = 0;

                // suppose it is always > 2 items
                var prev = reader.NextItem();
                var cur = reader.NextItem();
                while (cur != null)
                {
                    itemsCnt ++;

                    var cmp = comparer.Compare(prev, cur);
                    if (cmp == 0)
                    {
                        overallRepeatCnt ++;
                        seqRepeatCnt ++;
                    }
                    else
                    {
                        maxSeqRepeatCnt = Math.Max(maxSeqRepeatCnt, seqRepeatCnt);
                        seqRepeatCnt = 0;

                        if (cmp > 0)
                            sortFaults ++;
                    }

                    prev = cur;
                    cur = reader.NextItem();
                }

                var t2 = DateTime.UtcNow;

                Console.WriteLine($"Overall spent '{t2.Subtract(t1)}'");
                Console.WriteLine($"Sort faults = {sortFaults}");
                Console.WriteLine($"Overall items = {itemsCnt}");
                Console.WriteLine($"Overall repeats = {overallRepeatCnt}");
                Console.WriteLine($"Overall max sequential repeat = {maxSeqRepeatCnt}");
            }
        }
    }
}
