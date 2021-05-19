using Ncodi.CodeAnalysis.Symbols;
using Ncodi.CodeAnalysis.Syntax;
using System;

namespace Ncodi.CodeAnalysis.Binding
{
    internal sealed class BoundBinaryOperator
    {
        private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, TypeSymbol type)
            :this(syntaxKind,kind,type,type,type)
        {
        }

        private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, TypeSymbol operandType, TypeSymbol resultType)
    : this(syntaxKind, kind, operandType, operandType, resultType)
        {
        }
        private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, TypeSymbol leftType, TypeSymbol rightType, TypeSymbol resultType)
        {
            SyntaxKind = syntaxKind;
            Kind = kind;
            LeftType = leftType;
            RightType = rightType;
            Type = resultType;
        }

        public SyntaxKind SyntaxKind { get; }
        public BoundBinaryOperatorKind Kind { get; }
        public TypeSymbol LeftType { get; }
        public TypeSymbol RightType { get; }
        public TypeSymbol OperandType { get; }
        public TypeSymbol Type { get; }

        private static BoundBinaryOperator[] _operators =
        {
            new BoundBinaryOperator(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, TypeSymbol.Decimal),
            new BoundBinaryOperator(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, TypeSymbol.Decimal,TypeSymbol.Int,TypeSymbol.Decimal),
            new BoundBinaryOperator(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, TypeSymbol.Int,TypeSymbol.Decimal,TypeSymbol.Decimal),


            new BoundBinaryOperator(SyntaxKind.MinusToken, BoundBinaryOperatorKind.Substraction, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxKind.MinusToken, BoundBinaryOperatorKind.Substraction, TypeSymbol.Decimal),
            new BoundBinaryOperator(SyntaxKind.MinusToken, BoundBinaryOperatorKind.Substraction, TypeSymbol.Decimal,TypeSymbol.Int,TypeSymbol.Decimal),
            new BoundBinaryOperator(SyntaxKind.MinusToken, BoundBinaryOperatorKind.Substraction, TypeSymbol.Int,TypeSymbol.Decimal,TypeSymbol.Decimal),

            new BoundBinaryOperator(SyntaxKind.StarToken, BoundBinaryOperatorKind.Multiplication, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxKind.StarToken, BoundBinaryOperatorKind.Multiplication, TypeSymbol.Decimal),
            new BoundBinaryOperator(SyntaxKind.StarToken, BoundBinaryOperatorKind.Multiplication, TypeSymbol.Decimal,TypeSymbol.Int,TypeSymbol.Decimal),
            new BoundBinaryOperator(SyntaxKind.StarToken, BoundBinaryOperatorKind.Multiplication, TypeSymbol.Int,TypeSymbol.Decimal,TypeSymbol.Decimal),

            new BoundBinaryOperator(SyntaxKind.StarStarToken, BoundBinaryOperatorKind.Power, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxKind.StarStarToken, BoundBinaryOperatorKind.Power, TypeSymbol.Decimal),
            new BoundBinaryOperator(SyntaxKind.StarStarToken, BoundBinaryOperatorKind.Power, TypeSymbol.Decimal,TypeSymbol.Int,TypeSymbol.Decimal),
            new BoundBinaryOperator(SyntaxKind.StarStarToken, BoundBinaryOperatorKind.Power, TypeSymbol.Int,TypeSymbol.Decimal,TypeSymbol.Decimal),

            new BoundBinaryOperator(SyntaxKind.SlashSlashToken, BoundBinaryOperatorKind.EuclidianDivision, TypeSymbol.Int),

            new BoundBinaryOperator(SyntaxKind.SlashToken, BoundBinaryOperatorKind.DecimalDivision, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxKind.SlashToken, BoundBinaryOperatorKind.DecimalDivision, TypeSymbol.Decimal),
            new BoundBinaryOperator(SyntaxKind.SlashToken, BoundBinaryOperatorKind.DecimalDivision, TypeSymbol.Decimal,TypeSymbol.Int,TypeSymbol.Decimal),
            new BoundBinaryOperator(SyntaxKind.SlashToken, BoundBinaryOperatorKind.DecimalDivision, TypeSymbol.Int,TypeSymbol.Decimal,TypeSymbol.Decimal),

            new BoundBinaryOperator(SyntaxKind.ModuloToken, BoundBinaryOperatorKind.DivisionRemainder, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxKind.ModuloToken, BoundBinaryOperatorKind.DivisionRemainder, TypeSymbol.Decimal),
            new BoundBinaryOperator(SyntaxKind.ModuloToken, BoundBinaryOperatorKind.DivisionRemainder, TypeSymbol.Decimal,TypeSymbol.Int,TypeSymbol.Decimal),
            new BoundBinaryOperator(SyntaxKind.ModuloToken, BoundBinaryOperatorKind.DivisionRemainder, TypeSymbol.Int,TypeSymbol.Decimal,TypeSymbol.Decimal),

            new BoundBinaryOperator(SyntaxKind.AmpersandToken, BoundBinaryOperatorKind.BitwiseAnd, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxKind.PipeToken, BoundBinaryOperatorKind.BitwiseOr, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxKind.HatToken, BoundBinaryOperatorKind.BitwiseXor, TypeSymbol.Int),

            new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals, TypeSymbol.Decimal, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals, TypeSymbol.Decimal,TypeSymbol.Int,TypeSymbol.Decimal, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals, TypeSymbol.Int,TypeSymbol.Decimal,TypeSymbol.Decimal, TypeSymbol.Bool),

            new BoundBinaryOperator(SyntaxKind.BangEqualsToken, BoundBinaryOperatorKind.NotEquals, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.LessToken, BoundBinaryOperatorKind.LessThan, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.LessOrEqualsToken, BoundBinaryOperatorKind.LessThanOrEquals, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.GreaterToken, BoundBinaryOperatorKind.GreaterThan, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.GreaterOrEqualsToken, BoundBinaryOperatorKind.GreaterThanOrEquals, TypeSymbol.Int, TypeSymbol.Bool),

            new BoundBinaryOperator(SyntaxKind.LessToken,BoundBinaryOperatorKind.LessThan,TypeSymbol.Int,TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.LessOrEqualsToken,BoundBinaryOperatorKind.LessThanOrEquals,TypeSymbol.Int,TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.GreaterToken,BoundBinaryOperatorKind.GreaterThan,TypeSymbol.Int,TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.GreaterOrEqualsToken,BoundBinaryOperatorKind.GreaterThanOrEquals,TypeSymbol.Int,TypeSymbol.Bool),

            new BoundBinaryOperator(SyntaxKind.AmpersandToken, BoundBinaryOperatorKind.BitwiseAnd, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.AmpersandAmpersandToken, BoundBinaryOperatorKind.LogicalAnd, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.PipeToken, BoundBinaryOperatorKind.BitwiseOr, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.PipePipeToken, BoundBinaryOperatorKind.LogicalOr, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.HatToken, BoundBinaryOperatorKind.BitwiseXor, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxKind.BangEqualsToken, BoundBinaryOperatorKind.NotEquals, TypeSymbol.Bool),

            new BoundBinaryOperator(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, TypeSymbol.String),
            new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals, TypeSymbol.String),
            new BoundBinaryOperator(SyntaxKind.BangEqualsToken, BoundBinaryOperatorKind.NotEquals, TypeSymbol.String),
        };
        public static BoundBinaryOperator Bind(SyntaxKind syntaxKind, TypeSymbol leftType, TypeSymbol rightType)
        {
            foreach (var op in _operators)
            {
                if (op.SyntaxKind == syntaxKind && op.LeftType == leftType && op.RightType == rightType)
                {
                    return op;
                }
            }
            return null;
        }
    }
}
