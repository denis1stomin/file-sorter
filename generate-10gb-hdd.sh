#!/bin/bash

dotnet build -c Release ./FileGenerator/FileGenerator.csproj && \
    date && dotnet run -c Release -p ./FileGenerator/FileGenerator.csproj 10737418240 "$HDD_ROOT/dev/unsorted_10GB.txt" 2 && date
