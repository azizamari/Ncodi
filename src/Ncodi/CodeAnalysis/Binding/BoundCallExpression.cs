using Ncodi.CodeAnalysis.Symbols;
using Ncodi.CodeAnalysis.Text;
using System.Collections.Immutable;

namespace Ncodi.CodeAnalysis.Binding
{
    internal class BoundCallExpression : BoundExpression
    {
        public BoundCallExpression(FunctionSymbol function, ImmutableArray<BoundExpression> arguments)
        {
            Function = function;
            Arguments = arguments;
        }
        public BoundCallExpression(FunctionSymbol function, ImmutableArray<BoundExpression> arguments, TextLocation location)
        {
            Function = function;
            Arguments = arguments;
            Location = location;
        }

        public FunctionSymbol Function { get; }
        public ImmutableArray<BoundExpression> Arguments { get; }
        public TextLocation Location { get; }

        public override TypeSymbol Type => Function.ReturnType;

        public override BoundNodeKind Kind => BoundNodeKind.CallExpression;
    }
}