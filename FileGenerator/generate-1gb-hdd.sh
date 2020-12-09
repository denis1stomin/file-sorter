#!/bin/bash

dotnet build -c Release && date && dotnet run -c Release 1073741824 "$HDD_ROOT/dev/unsorted_1GB.txt" 1 && date
