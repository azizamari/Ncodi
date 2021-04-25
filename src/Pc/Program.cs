using Pital.CodeAnalysis;
using Pital.CodeAnalysis.Binding;
using Pital.CodeAnalysis.Syntax;
using Pital.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pc
{
    internal abstract class Repl
    {
        private StringBuilder _textBuilder =new StringBuilder();

        public void Run()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                if (_textBuilder.Length == 0)
                    Console.Write("» ");
                else
                    Console.Write("· ");
                Console.ResetColor();
                var input = Console.ReadLine();
                var isBlank = string.IsNullOrWhiteSpace(input);

                if (_textBuilder.Length == 0)
                {
                    if (isBlank)
                    {
                        break;
                    }
                    else if (input.StartsWith('#'))
                    {
                        EvaluateMetaCommand(input);
                        continue;
                    }

                }

                _textBuilder.AppendLine(input);
                var text = _textBuilder.ToString();
                if (!IsCompleteSubmission(text))
                    continue;
                EvaluateSubmission(text);
                _textBuilder.Clear();
            }
        }
        protected virtual void EvaluateMetaCommand(string input)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Invalid command {input}.");
            Console.ResetColor();
        }

        protected abstract bool IsCompleteSubmission(string text);

        protected abstract void EvaluateSubmission(string text);
    }

    internal sealed class PitalRepl : Repl
    {
        private Compilation _previous;
        private bool _showTree = false;
        private bool _showProgram = false;
        private readonly Dictionary<VariableSymbol, object> _variables = new Dictionary<VariableSymbol, object>();

        protected override void EvaluateMetaCommand(string input)
        {
            switch (input)
            {
                case "#showTree":
                    _showTree = !_showTree;
                    Console.WriteLine(_showTree ? "Showing parse trees." : "Not showing parse trees");
                    break;
                case "#showProgram":
                    _showProgram = !_showProgram;
                    Console.WriteLine(_showProgram ? "Showing bound tree" : "Not showing boud tree");
                    break;
                case "#cls":
                    Console.Clear();
                    break;
                case "#reset":
                    _previous = null;
                    break;
                default:
                    base.EvaluateMetaCommand(input);
                    break;
            }
        }

        protected override void EvaluateSubmission(string text)
        {
            var syntaxTree = SyntaxTree.Parse(text);


            var compilation = _previous == null ? new Compilation(syntaxTree) : _previous.ContinueWith(syntaxTree);
            var result = compilation.Evaluate(_variables);

            if (_showTree)
                syntaxTree.Root.WriteTo(Console.Out);
            Console.ResetColor();
            if (_showProgram)
                compilation.EmitTree(Console.Out);

            if (!result.Diagnostics.Any())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(result.Value);
                Console.ResetColor();

                _previous = compilation;
            }
            else
            {
                foreach (var diagnostic in result.Diagnostics)
                {
                    var lineIndex = syntaxTree.Text.GetLineIndex(diagnostic.Span.Start);
                    var line = syntaxTree.Text.Lines[lineIndex];
                    var lineNumber = lineIndex + 1;
                    var character = diagnostic.Span.Start - line.Start + 1;

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"({lineNumber}, {character}): ");
                    Console.WriteLine(diagnostic);
                    Console.ResetColor();

                    var prefixSpan = TextSpan.FromBounds(line.Start, diagnostic.Span.Start);
                    var suffixSpan = TextSpan.FromBounds(diagnostic.Span.End, line.End);

                    var prefix = syntaxTree.Text.ToString(prefixSpan);
                    var error = syntaxTree.Text.ToString(diagnostic.Span);
                    var suffix = syntaxTree.Text.ToString(suffixSpan);

                    Console.Write("    ");
                    Console.WriteLine(prefix + error + suffix);
                    var arrows = "    " + new string(' ', prefix.Length) + new string('^', error.Length);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(arrows);
                    Console.ResetColor();
                }

            }

            //return _previous;
        }

        protected override bool IsCompleteSubmission(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;
            var syntaxTree = SyntaxTree.Parse(text);
            if (syntaxTree.Diagnostics.Any())
                return false;
            return true;
        }
    }

    internal static class Program
    {
        private static void Main()
        {
            var repl = new PitalRepl();
            repl.Run();
        }
    }
}
