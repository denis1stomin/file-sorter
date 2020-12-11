#!/bin/bash

dotnet build -c Release && date && dotnet run -c Release 10737418240 "$HDD_ROOT/dev/unsorted_10GB.txt" 3 && date
