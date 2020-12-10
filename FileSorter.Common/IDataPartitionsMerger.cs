using System.IO;
using System.Text;

namespace FileSorter.Common
{
    public interface IDataPartitionsMerger<T> where T : class
    {
        /// A folder where partition files are saved to.
        DirectoryInfo PartitionFolder { get; }

        /// The name of a final destination file.
        string DestinationPath { get; }

        /// Used text encoding.
        Encoding Encoding { get; }

        /// Starts merging work.
        void StartWork(/* TODO ManualResetEvent noMorePartitionsEvent */);
    }
}
