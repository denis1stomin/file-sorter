using System;
using System.IO;
using System.Text;
using FileSorter.Common;

namespace FileGenerator
{
    public class TextualDataGenerator
    {
        public const int WriterBufferSize = 1024 * 1024 * 100;
        public static Encoding Encoding = Encoding.UTF8;
        public const string StrNumArr = "0123456789";
        public const string StrTextArr = " -ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789АБВГДЕЁЖЗИКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзиклмнопрстуфхцчшщъыьэюя";

        public static byte[] DelimeterBytes = Encoding.GetBytes(". ");
        public static byte[] NewLineBytes = Encoding.GetBytes(Environment.NewLine);

        public long GenerateAndWriteNewValue(ReadOnlySpan<char> sourceArr, int maxSize, FileStream stream)
        {
            var bytesWrittenBefore = stream.Position;
            var sourceArrLen = sourceArr.Length;

            int dataSize = _rand.Next(maxSize);
            Span<char> dataArr = stackalloc char[dataSize];

            for (int i = 0; i < dataSize; ++i)
            {
                int nextIdx = _rand.Next(sourceArrLen);
                dataArr[i] = sourceArr[nextIdx];
            }

            var cnt = Encoding.GetByteCount(dataArr);
            Span<byte> dataBytes = stackalloc byte[cnt];
            var encodedCnt = Encoding.GetBytes(dataArr, dataBytes);

            stream.Write(dataBytes);

            // TODO : it is not realy the same size as encoded bytes?
            var bytesWrittenAfter = stream.Position;
            return bytesWrittenAfter - bytesWrittenBefore;
        }

        public void Generate(string filePath, long maxFileSize)
        {
            // create possible char arrays
            Span<char> numArr = stackalloc char[StrNumArr.Length];
            for (int i = 0; i < numArr.Length; ++i)
                numArr[i] = StrNumArr[i];
            //
            Span<char> textArr = stackalloc char[StrTextArr.Length];
            for (int i = 0; i < textArr.Length; ++i)
                textArr[i] = StrTextArr[i];
            //
            Span<byte> delimArr = stackalloc byte[DelimeterBytes.Length];
            for (int i = 0; i < delimArr.Length; ++i)
                delimArr[i] = DelimeterBytes[i];
            //
            Span<byte> newLineArr = stackalloc byte[NewLineBytes.Length];
            for (int i = 0; i < newLineArr.Length; ++i)
                newLineArr[i] = NewLineBytes[i];

            const int maxNumberLength = 15;                 // long integer
            const int maxTextLength = 50;

            using (var output = Utils.CreateExclusiveWriteFile(filePath, WriterBufferSize))
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
                        output.Write(delimArr);

                        // text
                        bytesWritten += GenerateAndWriteNewValue(textArr, maxTextLength, output);

                        // new line
                        output.Write(newLineArr);
                    }
                }

                output.Close();
            }
        }

        private readonly Random _rand = new Random();
    }
}
