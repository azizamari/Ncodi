using Pital.CodeAnalysis.Symbols;
using System;

namespace Pital.CodeAnalysis.Binding
{
    internal abstract class BoundExpression : BoundNode
    {
        public abstract TypeSymbol Type { get; }
    }
}
