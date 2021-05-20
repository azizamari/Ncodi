using Ncodi.CodeAnalysis.Symbols;
using Ncodi.CodeAnalysis.Text;

namespace Ncodi.CodeAnalysis.Binding
{
    internal sealed class BoundStringIndexExpression : BoundExpression
    {
        public BoundStringIndexExpression(BoundExpression boundString, BoundExpression indexExpression, TextLocation location)
        {
            BoundString = boundString;
            IndexExpression = indexExpression;
            Location = location;
        }

        public override TypeSymbol Type => TypeSymbol.String;

        public override BoundNodeKind Kind => BoundNodeKind.StringIndexExpression;

        public BoundExpression BoundString { get; }
        public BoundExpression IndexExpression { get; }
        public TextLocation Location { get; }
    }
}