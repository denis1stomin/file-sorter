using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileSorter.Common;
using Moq;

namespace FileSorter.UTests
{
    [TestClass]
    public class DataPartitionerSorterTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BadConstructorArguments()
        {
            new DataPartitionerSorter<string>(null, "", 100500, null);
        }

        [TestMethod]
        public void StartWork()
        {
            ClearTestFolder();

            int maxCount = 100;
            var initialList = new List<string>();
            for (int i = 0; i< 3 * maxCount + 2; ++i)
                initialList.Add($"{_rand.Next(1000)}. Some text {_rand.Next(100)}");

            int idx = 0;
            var writer = new Mock<IDataReader<string>>();
            writer.Setup(x => x.NextItem())
                .Returns(() => {
                    if (idx >= initialList.Count)
                        return null;
                    return initialList[idx++];
                });

            var stringComparer = StringComparer.FromComparison(StringComparison.OrdinalIgnoreCase);
            var sorter = new DataPartitionerSorter<string>(writer.Object, TestFolder, maxCount, stringComparer);

            Assert.AreEqual(TestFolder, sorter.PartitionFolder);
            Assert.AreEqual(maxCount, sorter.PartitionMaxSize);

            sorter.StartWork();

            var partitionsCnt = Directory.GetFiles(TestFolder).Count();
            Assert.AreEqual(4, partitionsCnt);
        }

        private void ClearTestFolder()
        {
            if (Directory.Exists(TestFolder))
                Directory.Delete(TestFolder, true);
            
            Directory.CreateDirectory(TestFolder);
        }

        private Random _rand = new Random();
        private const string TestFolder = "./test-temp";
    }
}
