namespace FileSorter.Common
{
    public interface IDataReader //: IEnumerable<string>, IEnumerator<string>
    {
        DataItem NextItem();
    }
}
