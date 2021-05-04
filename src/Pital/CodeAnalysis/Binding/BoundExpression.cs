using Ncodi.CodeAnalysis.Symbols;
using System;

namespace Ncodi.CodeAnalysis.Binding
{
    internal abstract class BoundExpression : BoundNode
    {
        public abstract TypeSymbol Type { get; }
    }
}
