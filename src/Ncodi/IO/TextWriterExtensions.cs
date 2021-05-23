using Ncodi.CodeAnalysis.Syntax;
using Ncodi.CodeAnalysis.Text;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncodi.CodeAnalysis.IO
{
    public static class TextWriterExtensions
    {
        private static bool IsConsoleOut(this TextWriter writer)
        {
            if (writer == Console.Out)
                return true;

            if (writer is IndentedTextWriter iw && iw.InnerWriter.IsConsoleOut())
                return true;

            return false;
        }

        private static void SetForeground(this TextWriter writer, ConsoleColor color)
        {
            if (writer.IsConsoleOut())
                Console.ForegroundColor = color;
        }

        private static void ResetColor(this TextWriter writer)
        {
            if (writer.IsConsoleOut())
                Console.ResetColor();
        }

        public static void WriteKeyword(this TextWriter writer, SyntaxKind kind)
        {
            writer.WriteKeyword(SyntaxFacts.GetText(kind));
        }

        public static void WriteKeyword(this TextWriter writer, string text)
        {
            writer.SetForeground(ConsoleColor.Blue);
            writer.Write(text);
            writer.ResetColor();
        }

        public static void WriteIdentifier(this TextWriter writer, string text)
        {
            writer.SetForeground(ConsoleColor.DarkYellow);
            writer.Write(text);
            writer.ResetColor();
        }

        public static void WriteNumber(this TextWriter writer, string text)
        {
            writer.SetForeground(ConsoleColor.Cyan);
            writer.Write(text);
            writer.ResetColor();
        }
        
        public static void WriteString(this TextWriter writer, string text)
        {
            writer.SetForeground(ConsoleColor.Magenta);
            writer.Write(text);
            writer.ResetColor();
        }

        public static void WriteSpace(this TextWriter writer)
        {
            writer.WritePunctuation(" ");
        }

        public static void WritePunctuation(this TextWriter writer, SyntaxKind kind)
        {
            writer.WritePunctuation(SyntaxFacts.GetText(kind));
        }

        public static void WritePunctuation(this TextWriter writer, string text)
        {
            writer.SetForeground(ConsoleColor.DarkGray);
            writer.Write(text);
            writer.ResetColor();
        }
        public static void WriteDiagnostics(this TextWriter writer, IEnumerable<Diagnostic> diagnostics)
        {
            foreach (var diagnostic in diagnostics.OrderBy(d=>d.Location.Text.FileName).ThenBy(d => d.Location.Span.Start).ThenBy(d=>d.Location.Span.Length))
            {
                var text = diagnostic.Location.Text;
                var fileName = diagnostic.Location.FileName;
                var startLine = diagnostic.Location.StartLine + 1;
                var startCharacter = diagnostic.Location.StartCharacter + 1;
                //var endLine = diagnostic.Location.EndLine + 1;
                //var endCharacter = diagnostic.Location.EndCharacter + 1;

                var span = diagnostic.Location.Span;
                var lineIndex = text.GetLineIndex(span.Start);
                var line = text.Lines[lineIndex];

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"{fileName}(line {startLine}, col {startCharacter}): ");
                Console.WriteLine(diagnostic);
                Console.ResetColor();

                var prefixSpan = TextSpan.FromBounds(line.Start, span.Start);
                var suffixSpan = TextSpan.FromBounds(span.End, line.End);

                var prefix = text.ToString(prefixSpan);
                var error = text.ToString(span);
                var suffix = text.ToString(suffixSpan);


                Console.Write("  ");
                Console.WriteLine(prefix + error + suffix);
                var arrows = "  " + new string(' ', prefix.Length) + new string('^', error.Length);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(arrows);
                Console.ResetColor();
            }
        }
        public static string ReturnDiagnostics(this IEnumerable<Diagnostic> diagnostics)
        {
            var result = "";
            foreach (var diagnostic in diagnostics.OrderBy(d => d.Location.Text.FileName).ThenBy(d => d.Location.Span.Start).ThenBy(d => d.Location.Span.Length))
            {
                var text = diagnostic.Location.Text;
                var fileName = diagnostic.Location.FileName;
                var startLine = diagnostic.Location.StartLine + 1;
                var startCharacter = diagnostic.Location.StartCharacter + 1;

                var span = diagnostic.Location.Span;
                var lineIndex = text.GetLineIndex(span.Start);
                var line = text.Lines[lineIndex];

                result+=$"{fileName}(line {startLine}, col {startCharacter}): ";
                result+=diagnostic+"\n";

                var prefixSpan = TextSpan.FromBounds(line.Start, span.Start);
                var suffixSpan = TextSpan.FromBounds(span.End, line.End);

                var prefix = text.ToString(prefixSpan);
                var error = text.ToString(span);
                var suffix = text.ToString(suffixSpan);


                result+="  ";
                result += prefix + error + suffix;
                var arrows = "\n  " + new string(' ', prefix.Length) + new string('^', error.Length);
                result += arrows;
            }
            return result;
        }
    } 
}
