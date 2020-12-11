using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using FileSorter.Common;

namespace FileGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var startedAt = DateTime.UtcNow;

            ParseArgs(args, out var outputPath, out var fileSize, out var threadsNum, out var textualGen);

            // Just empirical coef to drop additional bytes of UTF-8 encoding.
            fileSize = (long)(fileSize * 0.983085069);

            if (textualGen)
            {
                Console.WriteLine($"Single thread mode without conversions.");

                var generator = new TextualDataGenerator();
                generator.Generate(outputPath, fileSize);

                return;
            }

            int fileBufferSize = 1024 * 1024 * 100;

            using (var writer = new SizedFileDataWriter<DataItem>(outputPath, fileSize, fileBufferSize))
            {
                if (threadsNum > 1)
                {
                    Console.WriteLine($"{threadsNum} threads mode.");

                    var threads = new List<Thread>(threadsNum);
                    for (int i = 0; i < threadsNum; ++i)
                    {
                        var threadStart = new ParameterizedThreadStart(GeneratorThreadFunc);
                        var thread = new Thread(threadStart);

                        thread.Start(writer);
                        threads.Add(thread);
                    }
                    
                    foreach (var thread in threads)
                        thread.Join();
                }
                else
                {
                    Console.WriteLine($"Single thread mode.");

                    var generator = new DataGenerator();
                    while (!writer.EnoughData())
                    {
                        var item = generator.NewItem();
                        writer.WriteItem(item);
                    }
                }
            }

            var spentTime = DateTime.UtcNow.Subtract(startedAt);

            var fileInfo = new FileInfo(outputPath);
            Console.WriteLine($"Generated {fileInfo.Length} bytes at '{outputPath}'.{Environment.NewLine}Spent '{spentTime}'.");
        }

        static void GeneratorThreadFunc(object writerInput)
        {
            var writer = writerInput as SizedFileDataWriter<DataItem>;
            if (writer == null)
            {
                Console.WriteLine("'writer' object is not of a type 'SizedFileDataWriter<DataItem>'.");
                Environment.Exit(1);
            }

            var generator = new DataGenerator();
            while (!writer.EnoughData())
            {
                var item = generator.NewItem();

                lock (writer)
                {
                    if (!writer.EnoughData())
                        writer.WriteItem(item);
                }
            }
        }

        static void ParseArgs(string[] args, out string outputPath, out long fileSize, out int threadsNum, out bool textualGen)
        {
            if (args.Length < 2)
                throw new ArgumentException("Not enough input parameters. Should be FileGenerator <sizeBytes> <outputPath>.");

            if (!long.TryParse(args[0], out fileSize))
                throw new ArgumentException("File size should be a valid long integer.");
            
            textualGen = false;
            outputPath = args[1];

            threadsNum = Environment.ProcessorCount / 2;
            if (args.Length > 2)
            {
                if (int.TryParse(args[2], out threadsNum))
                {
                    if (args.Length > 3)
                        textualGen = "textual".Equals(args[3]);
                }
                else
                    textualGen = "textual".Equals(args[2]);
            }
        }
    }
}
