using System;
using System.Text;
using System.Threading.Tasks;
using Pital.CodeAnalysis.Binding;
using Pital.CodeAnalysis.Syntax;

namespace Pital.CodeAnalysis
{
    internal class Evaluator
    {
        private readonly BoundExpression _root;

        public Evaluator(BoundExpression root)
        {
            _root = root;
        }
        public object Evaluate()
        {
            return EvaluateExpression(_root);
        }

        private object EvaluateExpression(BoundExpression node)
        {
            //binary
            //number
            if (node is BoundLiteralExpression n)
            {
                return n.Value;
            }

            if (node is BoundUnaryExpression u)
            {
                var operand = (int)EvaluateExpression(u.Operand);
                switch (u.OperatorKind)
                {
                    case BoundUnaryOperatorKind.Identity:
                        return operand;
                    case BoundUnaryOperatorKind.Negation:
                        return -operand;
                    default:
                        throw new Exception($"Unexpected Operator {u.OperatorKind}");
                }
            }

            if (node is BoundBinaryExpression b)
            {
                var left = (int)EvaluateExpression(b.Left);
                var right = (int)EvaluateExpression(b.Right);
                switch (b.OperatorKind)
                {
                    case BoundBinaryOperatorKind.Addition:
                        return left + right;
                    case BoundBinaryOperatorKind.Substraction:
                        return left - right;
                    case BoundBinaryOperatorKind.Multiplication:
                        return left * right;
                    case BoundBinaryOperatorKind.Division:
                        return left / right;
                    default:
                        throw new Exception($"Unexpected Operator {b.OperatorKind}");
                }
            }


            throw new Exception($"Unexpected node {node.Kind}");
        }
    }
}
