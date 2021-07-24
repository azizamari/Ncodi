namespace Ncodi.CodeAnalysis.Syntax
{
    internal class ParenthesizedIndexExpressionSyntax : ExpressionSyntax
    {
        public ParenthesizedIndexExpressionSyntax(SyntaxTree syntaxTree, ParenthesizedExpressionSyntax parenthesizedExpressionSyntax, SyntaxToken openBracket, ExpressionSyntax expression, SyntaxToken closedBracket)
            :base(syntaxTree)
        {
            ParenthesizedExpressionSyntax = parenthesizedExpressionSyntax;
            OpenBracket = openBracket;
            Expression = expression;
            ClosedBracket = closedBracket;
        }

        public ParenthesizedExpressionSyntax ParenthesizedExpressionSyntax { get; }
        public SyntaxToken OpenBracket { get; }
        public ExpressionSyntax Expression { get; }
        public SyntaxToken ClosedBracket { get; }

        public override SyntaxKind Kind => SyntaxKind.ParenthesizedIndexExpression;
    }
}