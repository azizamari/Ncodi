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
        EqualsEqualsToken,
        BangEqualsToken,
        BangToken,
        EqualsToken,
        LessOrEqualsToken,
        GreaterToken,
        GreaterOrEqualsToken,
        LessToken,
        OpenBraceToken,
        ClosedBraceToken,
        IdentifierToken,

        //Keywords
        TrueKeyword,
        FalseKeyword,
        ConstKeyword,
        VarKeyword,
        IfKeyword,
        ForKeyword,
        WhileKeyword,
        ElseKeyword,

        //Expressions
        LiteralExpression,
        BinaryExpression,
        ParenthesizedExpression,
        UnaryExpression,
        NameExpression,
        AssignmentExpression,

        //Nodes
        CompilationUnit,
        ElseClause,

        //Statements
        BlockStatement,
        ExpressionStatement,
        VariableDeclaration,
        IfStatement,
        WhileStatement,
        ForStatement,
        ToKeyword,
    }
}
