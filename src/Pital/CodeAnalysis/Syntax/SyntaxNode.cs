using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.IO;
using Pital.CodeAnalysis.Text;
using System;

namespace Pital.CodeAnalysis.Syntax
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }
        public virtual TextSpan Span
        {
            get
            {
                var first = GetChildren().First().Span;
                var last = GetChildren().Last().Span;
                return TextSpan.FromBounds(first.Start,last.End);
            }
        }

        public SyntaxToken GetLastToken()
        {
            if (this is SyntaxToken token)
                return token;

            // A syntax node should always contain at least 1 token.
            return GetChildren().Last().GetLastToken();
        }
        public IEnumerable<SyntaxNode> GetChildren()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach(var prop in properties)
            {
                if (typeof(SyntaxNode).IsAssignableFrom(prop.PropertyType))
                {
                    var child = (SyntaxNode)prop.GetValue(this);
                    if (child != null)
                        yield return child;
                }
                else if (typeof(SeparatedSyntaxList).IsAssignableFrom(prop.PropertyType))
                {
                    var separatedSyntaxList = (SeparatedSyntaxList)prop.GetValue(this);
                    foreach (var child in separatedSyntaxList.GetWithSeparators())
                        yield return child;
                }
                else if (typeof(IEnumerable<SyntaxNode>).IsAssignableFrom(prop.PropertyType))
                {
                    var children = (IEnumerable<SyntaxNode>)prop.GetValue(this);
                    foreach (var child in children)
                    {
                        if (child != null)
                            yield return child;
                    }
                }
            }
        }
        public void WriteTo(TextWriter writer)
        {
            TreePrint(writer, this);
        }

        private static void TreePrint(TextWriter writer, SyntaxNode node, string indent = "", bool isLast = true)
        {
            var isToConsole = writer == Console.Out;
            var marker = isLast ? "└──" : "├──";

            if (isToConsole)
                Console.ForegroundColor = ConsoleColor.DarkGray;

            writer.Write(indent);
            writer.Write(marker);

            if (isToConsole)
                Console.ForegroundColor = node is SyntaxToken ? ConsoleColor.Blue : ConsoleColor.Cyan;

            writer.Write(node.Kind);

            if (node is SyntaxToken t && t.Value != null)
            {
                writer.Write(" ");
                writer.Write(t.Value);
            }

            if (isToConsole)
                Console.ResetColor();

            writer.WriteLine();

            indent += isLast ? "   " : "│  ";

            var lastChild = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren())
                TreePrint(writer, child, indent, child == lastChild);
        }

        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                WriteTo(writer);
                return writer.ToString();
            }
        }

    

    }
}
