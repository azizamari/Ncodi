using Ncodi.CodeAnalysis.Symbols;
using System;

namespace Ncodi.CodeAnalysis.Binding
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
        public override TypeSymbol Type => Op.Type;

        public BoundExpression Right { get; }
        public BoundBinaryOperator Op { get; }
        public BoundExpression Left { get; }
    }
}
