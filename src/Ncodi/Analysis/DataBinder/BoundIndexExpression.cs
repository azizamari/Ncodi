using Ncodi.CodeAnalysis.Symbols;
using Ncodi.CodeAnalysis.Text;

namespace Ncodi.CodeAnalysis.Binding
{
    internal sealed class BoundIndexExpression : BoundExpression
    {
        public BoundIndexExpression(BoundExpression boundExpression, BoundExpression indexExpression, TextLocation location)
        {
            BoundExpression = boundExpression;
            IndexExpression = indexExpression;
            Location = location;
        }

        public override TypeSymbol Type => TypeSymbol.String;

        public override BoundNodeKind Kind => BoundNodeKind.StringIndexExpression;

        public BoundExpression BoundExpression { get; }
        public BoundExpression IndexExpression { get; }
        public TextLocation Location { get; }
    }
}