namespace Ncodi.CodeAnalysis.Syntax
{
    internal class BreakStatementSyntax : StatementSyntax
    {
        public BreakStatementSyntax(SyntaxToken keyword)
        {
            Keyword = keyword;
        }

        public SyntaxToken Keyword { get; }

        public override SyntaxKind Kind => SyntaxKind.BreakStatement;
    }
}