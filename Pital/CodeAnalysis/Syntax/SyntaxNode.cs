using System.Collections.Generic;
using System.Reflection;
using System.Linq;

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
        public IEnumerable<SyntaxNode> GetChildren() 
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach(var prop in properties)
            {
                if (typeof(SyntaxNode).IsAssignableFrom(prop.PropertyType))
                {
                    var child = (SyntaxNode)prop.GetValue(this);
                    yield return child;
                }
                else if (typeof(IEnumerator<SyntaxNode>).IsAssignableFrom(prop.PropertyType))
                {
                    var children = (IEnumerable<SyntaxNode>)prop.GetValue(this);
                    foreach (var child in children)
                        yield return child;

                }
            }
        }
    }
}
