using System;

namespace FileSorter.Common
{
    public interface IDataReader<T> : IDisposable //: IEnumerable<string>, IEnumerator<string>
    {
        T NextItem();
    }
}
