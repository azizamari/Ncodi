using System.Collections.Generic;

namespace Ncodi.CodeAnalysis.Syntax
{
    public sealed class UnaryExpressionSyntax : ExpressionSyntax
    {
        public UnaryExpressionSyntax(SyntaxToken operatorToken, ExpressionSyntax operand)
        {
            OperatorToken = operatorToken;
            Operand = operand;
        }

        public SyntaxToken OperatorToken { get; }
        public ExpressionSyntax Operand { get; }

        public override SyntaxKind Kind => SyntaxKind.UnaryExpression;

    }
}
