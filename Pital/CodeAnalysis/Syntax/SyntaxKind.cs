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

        //Expressions
        LiteralExpression,
        BinaryExpression,
        ParenthesizedExpression,
        UnaryExpression
    }
}
