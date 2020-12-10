#include <stdio.h>
#include <stdlib.h>
#include <wchar.h>
#include <time.h>

long GenerateAndWriteNewValue(const wchar_t* arr, const int arrLen, const int maxSize, FILE* file)
{
    const long bytesWrittenBefore = ftell(file);

    int dataSize = rand() % maxSize;
    while (dataSize > 0)
    {
        const int nextIdx = rand() % arrLen;

        const wchar_t el = arr[nextIdx];
        //const long elSize = sizeof(wchar_t);
        fwprintf(file, L"%c", el);

        //bytesWritten += elSize;
        --dataSize;
    }

    // TODO : it is not realy the same size as encoded bytes?
    const long bytesWrittenAfter = ftell(file);

    return bytesWrittenAfter - bytesWrittenBefore;
}

int main()
{
    srand(time(NULL));

    const long maxFileSize = 1024 * 1024 * 1024;    // 1 GB
    const int maxNumberLength = 15;                 // long integer
    const int maxTextLength = 50;

    const wchar_t* numArr = L"0123456789";
    const int numArrLen = wcslen(numArr);
    //
    const wchar_t* textArr = L" -ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    const int textArrLen = wcslen(textArr);
    //printf("%ls\n", textArr);
    //printf("%d\n", textArrLen);

    FILE* outFile = fopen("./unsorted.txt", "w,ccs=UTF-8");
    if (outFile == NULL)
    {
        printf("Cannot open output file\n");
        return 1;
    }
    // TODO : tune buffer size
    const size_t bufSize = 1024 * 1024 * 100;
    void* buf = malloc(bufSize);
    if (setvbuf(outFile, buf, _IOFBF, bufSize) != 0)
    {
        printf("Cannot change buffering mode\n");
        return 1;
    }

    long bytesWritten = 0;
    while (bytesWritten < maxFileSize)
    {
        // number
        const long numBytesAdded = GenerateAndWriteNewValue(numArr, numArrLen, maxNumberLength, outFile);
        bytesWritten += numBytesAdded;

        // Generate text only if we have not empty number
        if (numBytesAdded > 0)
        {
            // delimeter
            fwprintf(outFile, L".");
            fwprintf(outFile, L" ");

            // text
            bytesWritten += GenerateAndWriteNewValue(textArr, textArrLen, maxTextLength, outFile);

            // new line
            fwprintf(outFile, L"\n");

            fflush(outFile);
        }
    }

    fflush(outFile);
    fclose(outFile);
    free(buf);

    return 0;
}
