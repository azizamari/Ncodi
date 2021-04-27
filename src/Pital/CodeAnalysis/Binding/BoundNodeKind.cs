namespace Pital.CodeAnalysis.Binding
{
    internal enum BoundNodeKind
    {
        //expressions
        UnaryExpression,
        LiteralExpression,
        BinaryExpression,
        AssignmentExpression,
        VariableExpression,
        ErrorExpression,

        //statements 
        BlockStatement,
        ExpressionStatement,
        VariableDeclaration,
        IfStatement,
        WhileStatement,
        ForStatement,
        LabelStatement,
        ConditionalGotoStatement,
        GotoStatement,
    }
}
