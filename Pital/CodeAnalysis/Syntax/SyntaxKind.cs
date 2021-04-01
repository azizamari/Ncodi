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
        IdentifierToken,

        //Keywords
        TrueKeyword,
        FalseKeyword,

        //Expressions
        LiteralExpression,
        BinaryExpression,
        ParenthesizedExpression,
        UnaryExpression,
    }
}
