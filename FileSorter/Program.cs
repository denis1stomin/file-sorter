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
            var parser = new Parser(x => {
                x.CaseSensitive = false;
                x.IgnoreUnknownArguments = true;
                x.AutoHelp = true;
            });

            parser.ParseArguments<CmdParam>(args)
                .WithParsed<CmdParam>(MainInner)
                .WithNotParsed(NotParsed);
        }

        static void MainInner(CmdParam param)
        {
            var sorter = new FC.FileSorter(param.Source, param.Destination);
            sorter.Sort();
        }

        static void NotParsed(IEnumerable<Error> err)
        {
            Console.WriteLine("FileSorter -s <source-path> [-d <dest-path>] [-t <temp-folder>] [-p <partition-size>]");
            Environment.Exit(1);
        }
    }
}
