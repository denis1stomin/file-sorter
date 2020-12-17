using CommandLine;

namespace FileSorter
{
    class CmdParam
    {
        [Option('s', "source", Required = true, HelpText = "Path to a source data file to sort.")]
        public string Source { get; set; }

        [Option('d', "dest", Default = "sorted.txt", Required = false, HelpText = "Path to a sorted destination file.")]
        public string Destination { get; set; }

        [Option('t', "temp", Default = ".srt_tmp", Required = false, HelpText = "The path to a temp working folder.")]
        public string TempFolder { get; set; }

        [Option('p', "partition-size", Default = 1024 * 1024 * 250, Required = false, HelpText = "Maximum temporary file size in bytes.")]
        public long PartitionMaxSize { get; set; }

        [Option('r', "partitioning-threads", Default = 3, Required = false, HelpText = "A number of threads for the first algorithm step - partioning and sorting.")]
        public int PartitioningThreadsNum { get; set; }
    }
}
