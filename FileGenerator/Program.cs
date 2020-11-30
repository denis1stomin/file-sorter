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

            var generator = new DataGenerator();
            var encoding = Encoding.UTF8;

            using (var writer = new SizedFileDataWriter(outputPath, fileSize))
            {
                while (!writer.EnoughData())
                {
                    var item = generator.NewItem();
                    writer.WriteItem(item);
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
