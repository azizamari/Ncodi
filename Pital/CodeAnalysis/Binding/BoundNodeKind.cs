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

        //statements 
        BlockStatement,
        ExpressionStatement,
        VariableDeclaration,
        IfStatement,
        WhileStatement
    }
}
