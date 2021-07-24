namespace Ncodi.CodeAnalysis.Syntax
{
    public sealed class StringIndexExpressionSyntax : ExpressionSyntax
    {
        public StringIndexExpressionSyntax(SyntaxTree syntaxTree, ExpressionSyntax stringExpression, SyntaxToken openBracket, ExpressionSyntax expression, SyntaxToken closedBracket)
            : base(syntaxTree)
        {
            StringExpression = stringExpression;
            OpenBracket = openBracket;
            Expression = expression;
            ClosedBracket = closedBracket;
        }

        public override SyntaxKind Kind => SyntaxKind.StringIndexExpression;

        public ExpressionSyntax StringExpression { get; }
        public SyntaxToken OpenBracket { get; }
        public ExpressionSyntax Expression { get; }
        public SyntaxToken ClosedBracket { get; }
    }
}