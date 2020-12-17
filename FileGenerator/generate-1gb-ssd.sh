#!/bin/bash

dotnet build -c Release && date && dotnet run -c Release 1073741824 "./unsorted_1GB.txt" 3 && date
