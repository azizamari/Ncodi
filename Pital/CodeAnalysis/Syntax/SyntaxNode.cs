using System.Collections.Generic;

namespace Pital.CodeAnalysis.Syntax
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }
        public abstract IEnumerable<SyntaxNode> GetChildren();
    }
}
