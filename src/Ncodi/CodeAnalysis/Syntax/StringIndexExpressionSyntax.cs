namespace Ncodi.CodeAnalysis.Syntax
{
    public sealed class StringIndexExpressionSyntax : ExpressionSyntax
    {
        public StringIndexExpressionSyntax(SyntaxTree syntaxTree, SyntaxToken stringToken, SyntaxToken openBracket, ExpressionSyntax expression, SyntaxToken closedBracket)
            : base(syntaxTree)
        {
            StringToken = stringToken;
            OpenBracket = openBracket;
            Expression = expression;
            ClosedBracket = closedBracket;
        }

        public override SyntaxKind Kind => SyntaxKind.StringIndexExpression;

        public SyntaxToken StringToken { get; }
        public SyntaxToken OpenBracket { get; }
        public ExpressionSyntax Expression { get; }
        public SyntaxToken ClosedBracket { get; }
    }
}