using Ncodi.CodeAnalysis.Symbols;

namespace Ncodi.CodeAnalysis.Binding
{
    internal sealed class BoundErrorExpression: BoundExpression
    {
        public override TypeSymbol Type => TypeSymbol.Error;

        public override BoundNodeKind Kind => BoundNodeKind.ErrorExpression;
    }
}
