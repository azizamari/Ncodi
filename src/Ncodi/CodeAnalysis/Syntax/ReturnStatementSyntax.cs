namespace Ncodi.CodeAnalysis.Syntax
{
    internal class ReturnStatementSyntax : StatementSyntax
    {
        public ReturnStatementSyntax(SyntaxTree syntaxTree, SyntaxToken returnKeyword, ExpressionSyntax expression)
            : base(syntaxTree)
        {
            ReturnKeyword = returnKeyword;
            Expression = expression;
        }

        public SyntaxToken ReturnKeyword { get; }
        public ExpressionSyntax Expression { get; }

        public override SyntaxKind Kind => SyntaxKind.ReturnStatement;
    }
}