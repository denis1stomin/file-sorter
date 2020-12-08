using System;
using CommandLine;
using FC = FileSorter.Common;

namespace FileSorter
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new CommandLine.Parser(x => {
                x.CaseSensitive = false;
                x.IgnoreUnknownArguments = true;
            });

            var cmdParam = parser.ParseArguments<CmdParam>(args);

            var sorter = new FC.FileSorter("./unsorted_data.txt", "./output.txt");
            sorter.Sort();
        }
    }
}
