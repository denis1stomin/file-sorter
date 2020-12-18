#!/bin/bash

dotnet build -c Release ./FileGenerator/FileGenerator.csproj && \
    date && dotnet run -c Release -p ./FileGenerator/FileGenerator.csproj 1073741824 "$SSD_ROOT/dev/unsorted_1GB.txt" 4 && date
