namespace FileSorter.Common
{
    public interface IStreamSorter<T>
    {
        /// Current number of kept elements internally.
        int Count { get; }

        /// a maximum number of kept elements internally.
        int MaxCount { get; }

        /// Adds an element with respect to the sort order.
        bool Add(T item);

        /// Saves sorted elements into the appropriate output.
        void Save();
    }
}
