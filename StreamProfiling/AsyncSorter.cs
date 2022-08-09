using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamProfiling.AsyncSoorter
{
    internal class ReaderState
    {
        public Line Line { get; set; }

        public StreamReader Reader { get; set; }
    }

    internal record Line : IComparable<Line>
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

    internal static class AsyncSorter
    {
        public static async Task SortFile(string[] files)
        {
            await SortParts(files);
            await SortResult(files);
        }

        private static async Task SortParts(string[] files)
        {
            await Task.WhenAll(files.AsParallel().Select(async file =>
                {
                    var sortedLines = File.ReadAllLines(file)
                        .Select(x => new Line(x))
                        .OrderBy(x => x);
                    await File.WriteAllLinesAsync(file, sortedLines.Select(x => x.Build()));
                }
            ));
        }

        private static async Task SortResult(string[] files)
        {
            var readers = files.Select(x => new StreamReader(x));
            try
            {
                var lines = (await Task.WhenAll(readers.AsParallel().Select(async x => new ReaderState
                {
                    Line = new Line(await x.ReadLineAsync()),
                    Reader = x
                }))).ToList();

                using var writer = new StreamWriter("AsyncResult.txt");

                while (lines.Count != 0)
                {
                    var current = lines.OrderBy(x => x.Line).First();
                    
                    await writer.WriteLineAsync(current.Line.Build());

                    if (current.Reader.EndOfStream)
                    {
                        current.Reader.Close();
                        lines.Remove(current);
                    }
                    else
                    {
                        current.Line = new Line(await current.Reader.ReadLineAsync());
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
