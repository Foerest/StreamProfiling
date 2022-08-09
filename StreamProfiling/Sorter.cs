using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamProfiling
{
    internal class ReaderState
    {
        public Line Line { get; set; }

        public StreamReader Reader { get; set; }
    }

    internal class Line : IComparable<Line>
    {
        public Line(string line)
        {
            int pos = line.IndexOf(".");
            Number = int.Parse(line.Substring(0, pos));
            Word = line.Substring(pos + 2);
        }
        
        public int Number { get; set; }
        public string Word { get; set; }

        public string Build() => $"{Number}. {Word}";

        public int CompareTo(Line other)
        {
            int result = Word.CompareTo(other.Word);
            if (result != 0)
            {
                return result;
            }

            return Number.CompareTo(other.Number);
        }
    }

    internal static class Sorter
    {
        public static void SortFile(string[] files)
        {
            SortParts(files);
            SortResult(files);
        }

        private static void SortParts(string[] files)
        {
            foreach (var file in files)
            {
                var sortedLines = File.ReadAllLines(file)
                    .Select(x => new Line(x))
                    .OrderBy(x => x);
                File.WriteAllLines(file, sortedLines.Select(x => x.Build()));
            }
        }

        private static void SortResult(string[] files)
        {
            var readers = files.Select(x => new StreamReader(x));
            try
            {
                var lines = readers.Select(x => new ReaderState
                {
                    Line = new Line(x.ReadLine()),
                    Reader = x
                }).ToList();

                using var writer = new StreamWriter("result.txt");
                while (lines.Count != 0)
                {
                    var current = lines.OrderBy(x => x.Line).First();
                    
                    writer.WriteLine(current.Line.Build());

                    if (current.Reader.EndOfStream)
                    {
                        current.Reader.Close();
                        lines.Remove(current);
                    }
                    else
                    {
                        current.Line = new Line(current.Reader.ReadLine());
                    }
                }
            }
            finally
            {
                foreach (var r in readers)
                {
                    r.Dispose();
                }
            }
        }
    }
}
