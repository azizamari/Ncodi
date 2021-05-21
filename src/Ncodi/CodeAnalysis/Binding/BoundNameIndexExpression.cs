using Ncodi.CodeAnalysis.Symbols;
using Ncodi.CodeAnalysis.Text;

namespace Ncodi.CodeAnalysis.Binding
{
    internal sealed class BoundNameIndexExpression : BoundExpression
    {

        public BoundNameIndexExpression(BoundExpression boundName, BoundExpression indexExpression, TextLocation location)
        {
            BoundName = boundName;
            IndexExpression = indexExpression;
            Location = location;
        }

        public BoundExpression BoundName { get; }
        public BoundExpression IndexExpression { get; }
        public TextLocation Location { get; }

        public override TypeSymbol Type => TypeSymbol.String;

        public override BoundNodeKind Kind => BoundNodeKind.NameIndexExpression;
    }
}