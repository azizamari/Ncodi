﻿namespace Ncodi.CodeAnalysis.Binding
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
        CallExpression,

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
        ConversionExpression,
        DoWhileStatement,
        ReturnStatement,
        StringIndexExpression,
        NameIndexExpression,
    }
}
