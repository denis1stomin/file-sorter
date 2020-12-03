using System;
using System.Text;
using FileSorter.Common;

namespace FileGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            ParseArgs(args, out var outputPath, out var fileSize);

            using (var writer = new SizedFileDataWriter<DataItem>(outputPath, fileSize))
            {
                // TODO : Super simple way to improve the performance here is to 
                //        run this loop inside a number of separate threads and just synchronize output writer.
                //        Lines order is not important here, so no need in additional synchronizations.

                // new ThreadStart(writer)
                {
                    var generator = new DataGenerator();
                    while (!writer.EnoughData())
                    {
                        var item = generator.NewItem();

                        //lock (writer)
                        writer.WriteItem(item);
                    }
                }
            }
        }

        static void ParseArgs(string[] args, out string outputPath, out long fileSize)
        {
            if (args.Length < 2)
                throw new ArgumentException("Not enough input parameters. Should be FileGenerator <sizeBytes> <outputPath>.");

            if (!long.TryParse(args[0], out fileSize))
                throw new ArgumentException("File size should be a valid long integer.");
            
            outputPath = args[1];
        }
    }
}
