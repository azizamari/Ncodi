namespace Pital.CodeAnalysis
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
        ParenthesizedExpression
    }
}
