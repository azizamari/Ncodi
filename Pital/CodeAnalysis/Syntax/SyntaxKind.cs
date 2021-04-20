namespace Pital.CodeAnalysis.Syntax
{
    public enum SyntaxKind
    {
        //Tokens
        BadToken,
        EndOfFileToken,
        WhiteSpaceToken,
        NumberToken,
        PlusToken,
        StarToken,
        MinusToken,
        SlashToken,
        OpenParenthesisToken,
        ClosedParenthesisToken,
        AmpersandToken,
        PipeToken,
        IdentifierToken,
        EqualsEqualsToken,
        BangEqualsToken,
        BangToken,
        EqualsToken,
        OpenBraceToken,
        ClosedBraceToken,

        //Keywords
        TrueKeyword,
        FalseKeyword,

        //Expressions
        LiteralExpression,
        BinaryExpression,
        ParenthesizedExpression,
        UnaryExpression,
        NameExpression,
        AssignmentExpression,

        //Nodes
        CompilationUnit,

        //Statements
        BlockStatement,
        ExpressionStatement,
    }
}
