using Ncodi.CodeAnalysis.Symbols;
using Ncodi.CodeAnalysis.Text;

namespace Ncodi.CodeAnalysis.Binding
{
    internal sealed class BoundConversionExpression : BoundExpression
    {
        public BoundConversionExpression(TypeSymbol type, BoundExpression expression, TextLocation location)
        {
            Type = type;
            Expression = expression;
            Location = location;
        }

        public override TypeSymbol Type { get; }
        public BoundExpression Expression { get; }
        public TextLocation Location { get; }

        public override BoundNodeKind Kind => BoundNodeKind.ConversionExpression;
    }
}