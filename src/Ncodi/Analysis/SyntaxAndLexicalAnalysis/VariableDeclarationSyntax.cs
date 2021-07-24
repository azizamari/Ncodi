﻿namespace Ncodi.CodeAnalysis.Syntax
{
    public sealed class VariableDeclarationSyntax : StatementSyntax
    {
        public VariableDeclarationSyntax(SyntaxTree syntaxTree,SyntaxToken keywordToken, SyntaxToken identifier, TypeClauseSyntax typeClause, SyntaxToken equalsToken,ExpressionSyntax initializer)
            : base(syntaxTree)
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
