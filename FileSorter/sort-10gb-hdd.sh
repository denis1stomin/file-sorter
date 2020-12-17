#!/bin/bash

RELEASE_APP=./bin/Release/netcoreapp3.1/FileSorter
#PARTITION_SIZE=2097152
PARTITION_SIZE=125829120
#PARTITION_SIZE=262144000
#PARTITION_SIZE=671859200

dotnet build -c Release
date
"$RELEASE_APP" -s "$HDD_ROOT/dev/unsorted_10GB.txt" -d "$HDD_ROOT/dev/sorted_10GB.txt" -t "$HDD_ROOT/dev/tmp" -p $PARTITION_SIZE -r 6
date
