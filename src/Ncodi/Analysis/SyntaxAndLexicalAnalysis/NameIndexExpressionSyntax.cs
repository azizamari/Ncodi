namespace Ncodi.CodeAnalysis.Syntax
{
    internal sealed class NameIndexExpressionSyntax : ExpressionSyntax
    {

        public NameIndexExpressionSyntax(SyntaxTree syntaxTree, NameExpressionSyntax nameExpression, SyntaxToken openBracket, ExpressionSyntax expression, SyntaxToken closedBracket)
            :base(syntaxTree)
        {
            NameExpression = nameExpression;
            OpenBracket = openBracket;
            Expression = expression;
            ClosedBracket = closedBracket;
        }
        public NameExpressionSyntax NameExpression { get; }
        public SyntaxToken OpenBracket { get; }
        public ExpressionSyntax Expression { get; }
        public SyntaxToken ClosedBracket { get; }

        public override SyntaxKind Kind => SyntaxKind.NameIndexExpressoin;
    }
}