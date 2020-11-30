using System;
using CommandLine;

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

            
        }
    }
}
