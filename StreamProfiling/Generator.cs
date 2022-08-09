using System;
using System.IO;
using System.Linq;

namespace StreamProfiling
{
    internal static class Generator
    {
        private static readonly Random random = new();
        private static readonly string[] words;

        static Generator()
        {
            words = Enumerable.Range(0, 100_000)
                .Select(x =>
                {
                    var range = Enumerable.Range(0, random.Next(20, 100));
                    var chars = range.Select(x => (char)random.Next('A', 'Z')).ToArray();
                    var str = new string(chars);
                    return str;
                }).ToArray();
        }

        public static string Generate(int linesCount)
        {
            var fileName = "L" + linesCount + ".txt";

            using (var writer = new StreamWriter(fileName))
            {
                for (int i = 0; i < linesCount; i++)
                {
                    writer.WriteLine($"{GenerateNumber()}. {GenerateString()}");
                }
            }

            return fileName;
        }

        private static string GenerateString() => words[random.Next(0, words.Length)];

        private static string GenerateNumber() => random.Next(0, 10000).ToString();
    }
}
