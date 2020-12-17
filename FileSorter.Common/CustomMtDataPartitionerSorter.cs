using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace FileSorter.Common
{
    public class CustomMtDataPartitionerSorter : IDataPartitionerSorter<DataItem>
    {
        public int ThreadsMax { get; } = Environment.ProcessorCount;
        public int PartitionMapSize { get; } = 1024 * 1024 * 100;
        public string SourcePath { get; }
        public string PartitionFolder { get; }
        public static Encoding Encoding { get; } = Encoding.UTF8;

        public CustomMtDataPartitionerSorter(
            string sourcePath, string partitionFolder, int threadsMax, int mapSize)
        {
            ThreadsMax = (threadsMax > 0) ? threadsMax : throw new ArgumentException(nameof(threadsMax));
            PartitionMapSize = (mapSize > 0) ? mapSize : throw new ArgumentException(nameof(mapSize));

            SourcePath = !string.IsNullOrWhiteSpace(sourcePath) ? sourcePath
                : throw new ArgumentException(nameof(sourcePath));

            PartitionFolder = !string.IsNullOrWhiteSpace(partitionFolder) ? partitionFolder
                : throw new ArgumentException(nameof(partitionFolder));
        }

        public void StartWork(bool wait = true)
        {
            var partition = new List<DataMapEntry>((int)PartitionMapSize);

            var stream = Utils.OpenSharedReadFile(SourcePath /* , ReaderBufferSize */ );
            using (var reader = new StreamReader(stream, Encoding))
            {
                var dataMap = new List<DataMapEntry>(PartitionMapSize);

                long nextLinePosition = reader.BaseStream.Position;
                string nextLine = reader.ReadLine();
                while (nextLine != null)
                {
                    while (_threads.Count >= ThreadsMax)
                        Thread.Sleep(50);

                    dataMap.Add(ParseMapFromLine(nextLine, nextLinePosition));

                    if (dataMap.Count == PartitionMapSize)
                    {
                        var t = new Thread(new ParameterizedThreadStart(ThreadFunc));

                        lock (_threads)
                            _threads.Add(t);
                        t.Start(dataMap);

                        dataMap = new List<DataMapEntry>(PartitionMapSize);
                    }

                    nextLinePosition = reader.BaseStream.Position;
                    nextLine = reader.ReadLine();
                }
            }

            if (wait)
            {
                while (_threads.Count > 0)
                    Thread.Sleep(50);
            }
        }

        public void WaitWorkFinished()
        {
            throw new NotImplementedException();
        }

        private void ThreadFunc(object param)
        {
            var mapList = param as List<DataMapEntry>;
            mapList.Sort(new DataMapEntryComparer());

            var output = Utils.CreateExclusiveWriteFile($"{PartitionFolder}/{mapList.Count}_{Guid.NewGuid()}.part");
            var source2 = Utils.OpenSharedReadFile(SourcePath);
            using (var writer = new StreamWriter(output, Encoding))
            using (var reader2 = new StreamReader(source2, Encoding))
            {
                foreach (var entry in mapList)
                {
                    reader2.BaseStream.Position = entry.Position;
                    var line = reader2.ReadLine();
                    writer.WriteLine(line);
                }
            }

            lock (_threads)
                _threads.Remove(Thread.CurrentThread);
        }

        private static DataMapEntry ParseMapFromLine(string line, long linePosition)
        {
            var separator = '.';

            var idx = line.IndexOf(separator);
            if (idx < 0)
                throw new FormatException($"Cannot find separator symbol '{separator}' in data line '{line}'.");

            var strNumber = line.Substring(0, idx);

            if (!long.TryParse(strNumber, out var number))
                throw new FormatException($"Cannot convert '{strNumber}' to long type.");

            return new DataMapEntry(number, linePosition);
        }

        private List<Thread> _threads = new List<Thread>();

        class DataMapEntry
        {
            public long Number;
            public long Position;

            public DataMapEntry(long number, long position)
            {
                Number = number;
                Position = position;
            }
        }

        class DataMapEntryComparer : IComparer<DataMapEntry>
        {
            public int Compare(DataMapEntry e1, DataMapEntry e2)
            {
                return Math.Sign(e1.Number - e2.Number);
            }
        }
    }
}
