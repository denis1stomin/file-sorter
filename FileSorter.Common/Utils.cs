using System;
using System.IO;

namespace FileSorter.Common
{
    public static class Utils
    {
        public static FileStream OpenSharedReadFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException(nameof(path));
            
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public static FileStream CreateExclusiveWriteFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException(nameof(path));
            
            return new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        }

        public static FileStream CreateExclusiveWriteFile(string path, int bufferSize)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException(nameof(path));
            
            return new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize);
        }
    }
}
