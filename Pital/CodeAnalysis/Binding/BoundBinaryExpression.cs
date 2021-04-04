using System;

namespace Pital.CodeAnalysis.Binding
{

    internal sealed class BoundBinaryExpression : BoundExpression
    {
        public BoundBinaryExpression(BoundExpression left, BoundBinaryOperator op, BoundExpression right)
        {
            Right = right;
            Op = op;
            Left = left;
        }


        public override BoundNodeKind Kind => BoundNodeKind.BinaryExpression;
        public override Type Type => Op.Type;

        public BoundExpression Right { get; }
        public BoundBinaryOperator Op { get; }
        public BoundExpression Left { get; }
    }
}
