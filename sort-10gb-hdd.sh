#!/bin/bash

RELEASE_APP=./FileSorter/bin/Release/netcoreapp3.1/FileSorter
#PARTITION_SIZE=2097152
#PARTITION_SIZE=125829120
#PARTITION_SIZE=262144000
PARTITION_SIZE=524288000
#PARTITION_SIZE=1048576000

dotnet build -c Release ./FileSorter/FileSorter.csproj && \
    date && "$RELEASE_APP" -s "$HDD_ROOT/dev/unsorted_10GB.txt" -d "$HDD_ROOT/dev/sorted_10GB.txt" -t "$HDD_ROOT/dev/tmp" -z $PARTITION_SIZE -p 4 -m 2 && date
