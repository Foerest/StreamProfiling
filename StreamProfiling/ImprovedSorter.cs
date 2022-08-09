using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamProfiling.ImprovedSort
{
    internal class ReaderState
    {
        public Line Line { get; set; }

        public StreamReader Reader { get; set; }
    }

    internal readonly struct Line : IComparable<Line>
    {
        private string line { get; init; }
        private int position { get; init; }

        public Line(string line)
        {
            this.line = line;
            this.position = line.IndexOf(".");
            Number = int.Parse(line.AsSpan(0, position));
        }

        public int Number { get; init; }
        public ReadOnlySpan<char> Word => line.AsSpan(position + 2);

        public string Build() => line;

        public int CompareTo(Line other)
        {
            int result = Word.CompareTo(other.Word, StringComparison.Ordinal);
            if (result != 0)
            {
                return result;
            }

            return Number.CompareTo(other.Number);
        }
    }

    internal static class ImprovedSorter
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

                using var writer = new StreamWriter("ImprovedResult.txt");
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
