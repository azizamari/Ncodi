using Pital.CodeAnalysis;
using Pital.CodeAnalysis.Binding;
using Pital.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pc
{
    internal static class Program
    {
        private static void Main()
        {
            
            var showTree = false;
            var variables = new Dictionary<VariableSymbol, object>();
            while (true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    return;
                if (line == "#showTree")
                {
                    showTree = !showTree;
                    Console.WriteLine(showTree ? "Showing parse trees." : "Not showing parse trees");
                    continue;
                }
                else if (line == "#cls")
                {
                    Console.Clear();
                    continue;
                }

                var syntaxTree = SyntaxTree.Parse(line);
                var compilation = new Compilation(syntaxTree);
                var result = compilation.Evaluate(variables);

                var diagnostics = result.Diagnostics;

                if (showTree)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    syntaxTree.Root.WriteTo(Console.Out);
                    Console.ResetColor();
                }

                if (!diagnostics.Any())
                {
                    Console.WriteLine(result.Value);
                }
                else
                {
                    var text = syntaxTree.Text;
                    foreach (var diagnostic in diagnostics)
                    {
                        var lineIndex = text.GetLineIndex(diagnostic.Span.Start);
                        var lineNumber = lineIndex + 1;
                        var character = diagnostic.Span.Start - text.Lines[lineIndex].Start+1;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"({lineNumber}, {character}): ");
                        Console.WriteLine(diagnostic);
                        Console.ResetColor();

                        var prefix = line.Substring(0,diagnostic.Span.Start);
                        var error = line.Substring(diagnostic.Span.Start, diagnostic.Span.Length);
                        var suffix = line.Substring(diagnostic.Span.End);

                        Console.Write("    ");
                        Console.Write(prefix);

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(error);
                        Console.ResetColor();

                        Console.Write(suffix);
                        Console.WriteLine();
                    }
                    Console.WriteLine();

                }
            }
        }
    }
}
