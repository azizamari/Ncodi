using Ncodi.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;

namespace Ncodi.Test.CodeAnalysis
{
    internal sealed class AnnotatedText
    {
        public AnnotatedText(string text, ImmutableArray<TextSpan> spans)
        {
            Text = text;
            Spans = spans;
        }

        public string Text { get; }
        public ImmutableArray<TextSpan> Spans { get; }
        public static AnnotatedText Parse(string text)
        {
            text = Unindent(text);
            var textBuilder = new StringBuilder();
            var spanBuilder = ImmutableArray.CreateBuilder<TextSpan>();
            var startStack = new Stack<int>();

            var position = 0;

            foreach (var c in text)
            {
                if (c == '[')
                {
                    startStack.Push(position);
                }
                else if (c == ']')
                {
                    if (startStack.Count == 0)
                    {
                        throw new ArgumentException("Too many ']' in Text", nameof(text));
                    }
                    var start = startStack.Pop();
                    var end = position;
                    var span = TextSpan.FromBounds(start, end);
                    spanBuilder.Add(span);
                }
                else
                {
                    position++;
                    textBuilder.Append(c);
                }
            }

            if (startStack.Count != 0)
            {
                throw new ArgumentException("Missing ']' in Text", nameof(text));
            }

            return new AnnotatedText(textBuilder.ToString(),spanBuilder.ToImmutable());
        }
        private static string Unindent(string text)
        {
            var lines = UnidentLines(text);

            return string.Join(Environment.NewLine, lines);
        }

        public static string[] UnidentLines(string text)
        {
            var lines = new List<string>();
            using (var reader = new StringReader(text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }
            var minIndentation = int.MaxValue;
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (line.Trim().Length == 0)
                {
                    lines[i] = String.Empty;
                    continue;
                }
                var indentation = line.Length - line.TrimStart().Length;
                minIndentation = Math.Min(minIndentation, indentation);
            }

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Length == 0)
                {
                    continue;
                }
                lines[i] = lines[i].Substring(minIndentation);
            }

            if (lines.Count > 0 && lines[0].Length == 0)
            {
                lines.RemoveAt(0);
            }
            if (lines.Count > 0 && lines[^1].Length == 0)
            {
                lines.RemoveAt(lines.Count - 1);
            }

            return lines.ToArray();
        }
    }
}
