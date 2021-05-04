using Ncodi.CodeAnalysis.Symbols;
using System;

namespace Ncodi.CodeAnalysis.Binding
{
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundUnaryOperator op, BoundExpression operand)
        {
            Op = op;
            Operand = operand;
        }

        public BoundUnaryOperator Op { get; }
        public BoundExpression Operand { get; }

        public override TypeSymbol Type => Op.Type;

        public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
    }
}
