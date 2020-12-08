using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileSorter.Common;
using Moq;

namespace FileSorter.UTests
{
    [TestClass]
    public class StreamSorterTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BadConstructorArguments()
        {
            new StreamSorter<string>(null);
        }

        [TestMethod]
        public void SortAndSave()
        {
            int maxCount = 100;
            var initialList = new List<int>();
            var savedList = new List<int>();

            var writer = new Mock<IDataWriter<int>>();
            writer.Setup(x => x.WriteItem(It.IsAny<int>()))
                .Callback<int>(i => savedList.Add(i));

            var sorter = new StreamSorter<int>(writer.Object, null, maxCount);

            Assert.AreEqual(0, sorter.Count);
            Assert.AreEqual(maxCount, sorter.MaxCount);

            for (int i = 0; i < maxCount; ++i)
            {
                var next = _rand.Next();

                initialList.Add(next);
                bool inserted = sorter.Add(next);

                Assert.IsTrue(inserted);
            }

            sorter.Save();

            Assert.AreEqual(initialList.Count, savedList.Count);

            initialList.Sort();
            CollectionAssert.AreEqual(initialList, savedList);
        }

        [TestMethod]
        public void ExceedMaximum()
        {
            int maxCount = 10;
            var writer = new Mock<IDataWriter<int>>();
            var sorter = new StreamSorter<int>(writer.Object, null, maxCount);

            Assert.AreEqual(0, sorter.Count);
            Assert.AreEqual(maxCount, sorter.MaxCount);

            for (int i = 0; i < maxCount; ++i)
            {
                bool inserted = sorter.Add(_rand.Next());
                Assert.IsTrue(inserted);
            }
            for (int i = 0; i < 5; ++i)
            {
                bool inserted = sorter.Add(_rand.Next());
                Assert.IsFalse(inserted);
            }
        }

        private Random _rand = new Random();
        private const string TestFilePath = "./test-output.txt";
    }
}
