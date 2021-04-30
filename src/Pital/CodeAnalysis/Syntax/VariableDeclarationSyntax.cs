namespace Pital.CodeAnalysis.Syntax
{
    public sealed class VariableDeclarationSyntax : StatementSyntax
    {
        public VariableDeclarationSyntax(SyntaxToken keywordToken, SyntaxToken identifier, TypeClauseSyntax typeClause, SyntaxToken equalsToken,ExpressionSyntax initializer)
        {
            KeywordToken = keywordToken;
            Identifier = identifier;
            TypeClause = typeClause;
            EqualsToken = equalsToken;
            Initializer = initializer;
        }
        public override SyntaxKind Kind => SyntaxKind.VariableDeclaration;

        public SyntaxToken KeywordToken { get; }
        public SyntaxToken Identifier { get; }
        public TypeClauseSyntax TypeClause { get; }
        public SyntaxToken EqualsToken { get; }
        public ExpressionSyntax Initializer { get; }
    }
}
