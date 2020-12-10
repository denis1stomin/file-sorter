#include <iostream>
#include <fstream>

int main()
{
    std::ofstream outfile("unsorted.txt");

    outfile << "bytes here" << std::endl;

    // TODO

    outfile.close();

    return 0;
}
