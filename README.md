# Big File Sorter (External-Sort alg)

Simple application to sort line entries in a large file.  

Language: C#  
Dev Env: dotnet core 3.1, ubuntu 18.04  

On Windows OS bash scripts should be run in some bash emulator or just run scripts content manually.  

To generate test data file run commands:  
`HDD_ROOT=<a path to HDD mount point>`  
`sh generate-10gb-hdd.sh`  

To sort data file run command:  
`sh sort-10gb-hdd.sh`  
CmdParam class provides possibility to change some parameters like partition size, number of partitioning or merging threads etc.  
It can be used to tune the application performance in the case of slow disk IO or small RAM size etc.  
Run `dotnet run -c Release -p ./FileSorter/FileSorter.csproj -h` to get help information.  

To check correct sort results run command:  
`sh check-file-is-sorted.sh`  

To run some simple unit tests do:  
`dotnet test ./FileSorter.UTests/FileSorter.UTests.csproj`
