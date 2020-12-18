#!/bin/bash

dotnet build -c Release ./FileGenerator/FileGenerator.csproj && \
    date && dotnet run -c Release -p ./FileGenerator/FileGenerator.csproj 1073741824 "$HDD_ROOT/dev/unsorted_1GB.txt" 2 && date
