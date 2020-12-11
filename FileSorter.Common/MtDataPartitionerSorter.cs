using System;
using System.Threading;
using System.Collections.Generic;

namespace FileSorter.Common
{
    public class MtDataPartitionerSorter<T> : DataPartitionerSorter<T> where T : class
    {
        public int ThreadsNum { get; } = Environment.ProcessorCount;

        public MtDataPartitionerSorter(
            IDataReader<T> dataReader, string partitionFolder, long partitionMaxSize, IComparer<T> dataComparer, int threadsNum)
            : base(dataReader, partitionFolder, partitionMaxSize, dataComparer)
        {
            ThreadsNum = (threadsNum > 0) ? threadsNum : throw new ArgumentException(nameof(threadsNum));
        }

        public override void StartWork(bool wait = true)
        {
            var threads = new List<Thread>(ThreadsNum);
            for (int i = 0; i< ThreadsNum; ++i)
            {
                var thread = new Thread(ThreadFunc);
                thread.Start();

                threads.Add(thread);
            }

            if (wait)
            {
                foreach (var thread in threads)
                    thread.Join();
            }
        }

        private void ThreadFunc()
        {
            // Looks like a binary tree
            // https://github.com/microsoft/referencesource/blob/master/System/compmod/system/collections/generic/sortedset.cs
            var data = new SortedSet<T>(_dataComparer);

            T next = null;
            do
            {
                lock (_dataReader)
                    next = _dataReader.NextItem();
                
                if (next != null)
                {
                    data.Add(next);

                    // TODO : currently _data.Count is not bytes!!!
                    if (data.Count >= PartitionMaxSize)
                        SavePartition(data, PartitionFolder);
                }
            }
            while (next != null);

            if (data.Count > 0)
                SavePartition(data, PartitionFolder);
        }
    }
}
