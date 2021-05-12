using Ncodi.CodeAnalysis;
using Ncodi.CodeAnalysis.Symbols;
using Ncodi.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ncodi
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine("Usage: nc <Source-Path>");
                return;
            }
            if (args.Length > 1)
            {
                Console.Error.WriteLine("Error: only one path supported for the time being");
                return;
            }

            var path = args.Single();
            try
            {
                var text = File.ReadAllText(path);
                var syntaxTree = SyntaxTree.Parse(text);

                var compilation = new Compilation(syntaxTree);
                var result = compilation.Evaluate(new Dictionary<VariableSymbol, object>());
            }
            catch (IOException)
            {
                Console.Error.WriteLine("Error: File Not Found");
                return;
            }
        }
    }
}
