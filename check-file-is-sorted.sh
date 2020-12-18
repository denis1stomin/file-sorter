#!/bin/bash

dotnet build -c Release ./SortChecker/SortChecker.csproj && \
    date && dotnet run -c Release -p ./SortChecker/SortChecker.csproj "$HDD_ROOT/dev/sorted_1GB.txt" && date
