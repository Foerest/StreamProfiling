using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamProfiling
{
    public static class Splitter
    {
        public static string[] SplitFile(string fileName, int partSize)
        {
            var list = new List<string>();

            using var reader = new StreamReader(fileName);
            var partNumber = 0;
            while (!reader.EndOfStream)
            {
                partNumber++;
                var partFileName = $"{partNumber}.txt";
                list.Add(partFileName);

                using var writer = new StreamWriter(partFileName);

                for (int i = 0; i < partSize && !reader.EndOfStream; i++)
                {
                    writer.WriteLine(reader.ReadLine());
                }
            }

            return list.ToArray();
        }
    }
}
