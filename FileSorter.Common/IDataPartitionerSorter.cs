using System.Text;

namespace FileSorter.Common
{
    public interface IDataPartitionerSorter<T>
    {
        /// Starts partitioning work.
        void StartWork(bool wait = true);

        /// Blocks current thread till whole work is finished.
        void WaitWorkFinished();
    }
}
