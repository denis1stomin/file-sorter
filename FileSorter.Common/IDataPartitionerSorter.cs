using System.Text;

namespace FileSorter.Common
{
    public interface IDataPartitionerSorter<T>
    {
        /// A folder where partition files are saved to.
        string PartitionFolder { get; }

        /// A maximum size of sorted partition.
        long PartitionMaxSize { get; }

        /// Text encoding for the handled stream.
        Encoding Encoding { get; }

        /// Starts partitioning work.
        void StartWork();
    }
}