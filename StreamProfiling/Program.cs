using System;
using System.Diagnostics;
using System.Threading.Tasks;
using StreamProfiling.AsyncSoorter;
using StreamProfiling.ImprovedSort;

namespace StreamProfiling
{
    class Program
    {
        // Task 1
        // Write the console app to generate file like
        //  Number. AlphabetLine
        //  Example:
        //      1234. ABCdeofgrevdfknDSAgfdnk
        //  Number in the range [0, 10000]
        //  Line length is in [20, 100]
        // File can be up to 200 GB
        // use Count of lines as an input
        
        
        // Task 2
        // Write a code to sort the file from the previous task. Performance and memory are critical
        

        static async Task Main(string[] args)
        {
            var inputFileName = Generator.Generate(1_000_000);

            var fileNames = Splitter.SplitFile(inputFileName, 100_000);

            var sw = Stopwatch.StartNew();
            Sorter.SortFile(fileNames);
            sw.Stop();
            Console.WriteLine($"Execution time for normal sorter {sw.Elapsed}");

            fileNames = Splitter.SplitFile(inputFileName, 100_000);
            sw.Restart();
            ImprovedSorter.SortFile(fileNames);
            sw.Stop();
            Console.WriteLine($"Execution time for improved sorter {sw.Elapsed}");

            fileNames = Splitter.SplitFile(inputFileName, 100_000);
            sw.Restart();
            await AsyncSorter.SortFile(fileNames).ConfigureAwait(false);
            sw.Stop();
            Console.WriteLine($"Execution time for async sorter {sw.Elapsed}");

            //Console.ReadLine();
        }
    }
}
