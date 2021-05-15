namespace Ncodi.CodeAnalysis.Syntax
{
    internal class ContinueStatementSyntax : StatementSyntax
    {
        public ContinueStatementSyntax(SyntaxTree syntaxTree, SyntaxToken keyword)
            : base(syntaxTree)
        {
            Keyword = keyword;
        }

        public SyntaxToken Keyword { get; }

        public override SyntaxKind Kind => SyntaxKind.ContinueStatement;
    }
}