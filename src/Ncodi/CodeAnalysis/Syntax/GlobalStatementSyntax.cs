namespace Ncodi.CodeAnalysis.Syntax
{
    public sealed class GlobalStatementSyntax: MemberSyntax
    {
        public GlobalStatementSyntax(SyntaxTree syntaxTree, StatementSyntax statement)
            : base(syntaxTree)
        {
            Statement = statement;
        }

        public StatementSyntax Statement { get; }

        public override SyntaxKind Kind => SyntaxKind.GlobalStatmenet;
    }
}
