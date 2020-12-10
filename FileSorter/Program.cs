using System;
using System.Collections.Generic;
using CommandLine;
using FC = FileSorter.Common;

namespace FileSorter
{
    class Program
    {
        static void Main(string[] args)
        {
            var p1 = DateTime.UtcNow;

            var parser = new Parser(x => {
                x.CaseSensitive = false;
                x.IgnoreUnknownArguments = true;
                x.AutoHelp = true;
            });

            parser.ParseArguments<CmdParam>(args)
                .WithParsed<CmdParam>(MainInner)
                .WithNotParsed(NotParsed);
            
            var p2 = DateTime.UtcNow;

            Console.WriteLine($"Overall spent '{p2.Subtract(p1)}'");
        }

        static void MainInner(CmdParam param)
        {
            var sorter = new FC.FileSorter(
                param.Source, param.Destination, param.TempFolder, param.PartitionMaxSize);

            sorter.Sort();
        }

        static void NotParsed(IEnumerable<Error> err)
        {
            Console.WriteLine("FileSorter -s <source-path> [-d <dest-path>] [-t <temp-folder>] [-p <partition-size>]");
            Environment.Exit(1);
        }
    }
}
