namespace FileSorter.Common
{
    public interface IDataWriter<T>
    {
        void WriteItem(T item);
    }
}
