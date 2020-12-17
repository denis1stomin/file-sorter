#!/bin/bash

RELEASE_APP=./bin/Release/netcoreapp3.1/FileSorter
#PARTITION_SIZE=2097152
PARTITION_SIZE=125829120

dotnet build -c Release
date
"$RELEASE_APP" -s "$HDD_ROOT/dev/unsorted_1GB.txt" -d "$HDD_ROOT/dev/sorted_1GB.txt" -t "$HDD_ROOT/dev/tmp" -p $PARTITION_SIZE -r 6
date
