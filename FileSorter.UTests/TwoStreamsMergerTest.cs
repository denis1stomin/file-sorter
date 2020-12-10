using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileSorter.Common;
using FileGenerator;

namespace FileSorter.UTests
{
    [TestClass]
    public class TwoStreamsMergerTest
    {
        [DataTestMethod]
        [DataRow(0, 100, 150)]
        [DataRow(200, 100, 150)]
        [DataRow(4321, 5678, 150)]
        public void Test1(int length1, int length2, long maxNumber)
        {
            var root = "/home/des/src/own/test-tasks/file-sorter/FileSorter.UTests/";
            var sourcePath1 = $"{root}input_{length1}.txt";
            var sourcePath2 = $"{root}input_{length2}.txt";
            var outputPath = $"{root}output_{length1}_{length2}.txt";

            var comparer = new DataItemTrickyComparer();

            var data1 = CreateTestData(length1, maxNumber);
            data1.Sort(comparer);
            WriteToFile(data1, sourcePath1);

            var data2 = CreateTestData(length2, maxNumber);
            data2.Sort(comparer);
            WriteToFile(data2, sourcePath2);

            var sourceFile = new FileStream(sourcePath1, FileMode.Open, FileAccess.Read, FileShare.Read);
            var sourceFile2 = new FileStream(sourcePath2, FileMode.Open, FileAccess.Read, FileShare.Read);
            var destFile = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);

            using(var merger = new TwoStreamsMerger<DataItem>(
                        sourceFile, sourceFile2, destFile, new DataItemTrickyComparer(), s => new DataItem(s)))
            {
                merger.Merge();
            }

            var wholeData = new List<DataItem>();
            wholeData.AddRange(data1);
            wholeData.AddRange(data2);
            wholeData.Sort(comparer);

            var expected = wholeData.Select(i => i.ToString()).ToList();
            var actual = File.ReadAllLines(outputPath, Encoding.UTF8).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        private List<DataItem> CreateTestData(int length, long maxNumber)
        {
            var generator = new DataGenerator();
            var result = new List<DataItem>();

            while (result.Count < length)
            {
                result.Add(generator.NewItem());
            }

            return result;
        }

        private void WriteToFile(IList<DataItem> data, string path)
        {
            var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                foreach (var item in data)
                    writer.WriteLine(item);
            }
        }
    }
}
