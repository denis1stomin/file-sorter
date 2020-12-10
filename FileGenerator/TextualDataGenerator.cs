using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using FileSorter.Common;

namespace FileGenerator
{
    public class TextualDataGenerator
    {
        public static Encoding Encoding = Encoding.UTF8;
        public static byte[] DelimeterBytes = Encoding.GetBytes(". ");
        public static byte[] NewLineBytes = Encoding.GetBytes(Environment.NewLine);

        public long GenerateAndWriteNewValue(string arr, int maxSize, FileStream stream)
            {
                var bytesWrittenBefore = stream.Position;
                var arrLen = arr.Length;

                int dataSize = _rand.Next(maxSize);
                while (dataSize > 0)
                {
                    int nextIdx = _rand.Next(arrLen);

                    var bytes = Encoding.GetBytes(arr, nextIdx, 1);
                    stream.Write(bytes);

                    --dataSize;
                }

                // TODO : it is not realy the same size as encoded bytes?
                var bytesWrittenAfter = stream.Position;

                return bytesWrittenAfter - bytesWrittenBefore;
            }

        public void Generate(string filePath, long maxFileSize)
        {
            string numArr = "0123456789";
            string textArr = " -ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            const int bufferSize = 1024 * 1024 * 100;
            const int maxNumberLength = 15;                 // long integer
            const int maxTextLength = 50;

            using (var output = Utils.CreateExclusiveWriteFile(filePath, bufferSize))
            {
                long bytesWritten = 0;
                while (bytesWritten < maxFileSize)
                {
                    // number
                    long numBytesAdded = GenerateAndWriteNewValue(numArr, maxNumberLength, output);
                    bytesWritten += numBytesAdded;

                    // Generate text only if we have not empty number
                    if (numBytesAdded > 0)
                    {
                        // delimeter
                        output.Write(DelimeterBytes);

                        // text
                        bytesWritten += GenerateAndWriteNewValue(textArr, maxTextLength, output);

                        // new line
                        output.Write(NewLineBytes);
                    }
                }

                output.Close();
            }
        }

        private Random _rand = new Random();
    }
}
