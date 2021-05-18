using System;
using System.Collections.Generic;

namespace Ncodi.CodeAnalysis.Syntax
{
    public static class SyntaxFacts
    {
        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.BangToken:
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.TildeToken:
                    return 6;
                default:
                    return 0;
            }
        }
        public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.StarToken:
                case SyntaxKind.StarStarToken:
                case SyntaxKind.SlashToken:
                case SyntaxKind.SlashSlashToken:
                case SyntaxKind.ModuloToken:
                    return 5;

                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 4;

                case SyntaxKind.EqualsEqualsToken:
                case SyntaxKind.BangEqualsToken:
                case SyntaxKind.LessToken:
                case SyntaxKind.LessOrEqualsToken:
                case SyntaxKind.GreaterToken:
                case SyntaxKind.GreaterOrEqualsToken:
                    return 3;

                case SyntaxKind.AmpersandAmpersandToken:
                case SyntaxKind.AmpersandToken:
                    return 2;

                case SyntaxKind.PipePipeToken:
                case SyntaxKind.PipeToken:
                case SyntaxKind.HatToken:
                    return 1;

                default:
                    return 0;
            }
        }

        public static SyntaxKind GetKeywordKind(string text)
        {
            switch (text)
            {
                case "true":
                    return SyntaxKind.TrueKeyword;
                case "false":
                    return SyntaxKind.FalseKeyword;
                case "const":
                    return SyntaxKind.ConstKeyword;
                case "var":
                    return SyntaxKind.VarKeyword;
                case "if":
                    return SyntaxKind.IfKeyword;
                case "else":
                    return SyntaxKind.ElseKeyword;
                case "while":
                    return SyntaxKind.WhileKeyword;
                case "for":
                    return SyntaxKind.ForKeyword;
                case "to":
                    return SyntaxKind.ToKeyword;
                case "do":
                    return SyntaxKind.DoKeyword;
                case "def":
                    return SyntaxKind.FunctionKeyword;
                case "break":
                    return SyntaxKind.BreakKeyword;
                case "continue":
                    return SyntaxKind.ContinueKeyword;
                case "return":
                    return SyntaxKind.ReturnKeyword;

                //tunisian syntax
                case "w":
                    return SyntaxKind.AmpersandAmpersandToken;
                case "wela":
                    return SyntaxKind.PipePipeToken;
                case "madem":
                    return SyntaxKind.WhileKeyword;
                case "kan":
                    return SyntaxKind.IfKeyword;
                case "makanech":
                    return SyntaxKind.ElseKeyword;
                case "dir":
                    return SyntaxKind.DoKeyword;
                case "o5rej":
                    return SyntaxKind.BreakKeyword;
                case "kamel":
                    return SyntaxKind.ContinueKeyword;
                default:
                    return SyntaxKind.IdentifierToken;
            }
        }

        public static IEnumerable<SyntaxKind> GetUnaryOperatorKinds()
        {
            var kinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));
            foreach (var kind in kinds)
            {
                if (GetUnaryOperatorPrecedence(kind) > 0)
                    yield return kind;
            }
        }

        public static IEnumerable<SyntaxKind> GetBinaryOperatorKinds()
        {
            var kinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));
            foreach (var kind in kinds)
            {
                if (GetBinaryOperatorPrecedence(kind) > 0)
                    yield return kind;
            }
        }

        public static string GetText(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PlusToken:
                    return "+";
                case SyntaxKind.MinusToken:
                    return "-";
                case SyntaxKind.StarToken:
                    return "*";
                case SyntaxKind.StarStarToken:
                    return "**";
                case SyntaxKind.SlashToken:
                    return "/";
                case SyntaxKind.ModuloToken:
                    return "%";
                case SyntaxKind.SlashSlashToken:
                    return "//";
                case SyntaxKind.BangToken:
                    return "!";
                case SyntaxKind.EqualsToken:
                    return "=";
                case SyntaxKind.AmpersandAmpersandToken:
                    return "&&";
                case SyntaxKind.PipePipeToken:
                    return "||";
                case SyntaxKind.AmpersandToken:
                    return "&";
                case SyntaxKind.PipeToken:
                    return "|";
                case SyntaxKind.HatToken:
                    return "^";
                case SyntaxKind.CommaToken:
                    return ",";
                case SyntaxKind.ColonToken:
                    return ":";
                case SyntaxKind.TildeToken:
                    return "~";
                case SyntaxKind.EqualsEqualsToken:
                    return "==";
                case SyntaxKind.BangEqualsToken:
                    return "!=";
                case SyntaxKind.LessToken:
                    return "<";
                case SyntaxKind.LessOrEqualsToken:
                    return "<=";
                case SyntaxKind.GreaterToken:
                    return ">";
                case SyntaxKind.GreaterOrEqualsToken:
                    return ">=";
                case SyntaxKind.OpenParenthesisToken:
                    return "(";
                case SyntaxKind.ClosedParenthesisToken:
                    return ")";
                case SyntaxKind.OpenBraceToken:
                    return "{";
                case SyntaxKind.ClosedBraceToken:
                    return "}";
                case SyntaxKind.FunctionKeyword:
                    return "def";
                case SyntaxKind.FalseKeyword:
                    return "false";
                case SyntaxKind.TrueKeyword:
                    return "true";
                case SyntaxKind.VarKeyword:
                    return "var";
                case SyntaxKind.ConstKeyword:
                    return "const";
                case SyntaxKind.IfKeyword:
                    return "if";
                case SyntaxKind.ElseKeyword:
                    return "else";
                case SyntaxKind.WhileKeyword:
                    return "while";
                case SyntaxKind.ForKeyword:
                    return "for";
                case SyntaxKind.ToKeyword:
                    return "to";
                case SyntaxKind.DoKeyword:
                    return "do";
                case SyntaxKind.ContinueKeyword:
                    return "continue";
                case SyntaxKind.BreakKeyword:
                    return "break";
                case SyntaxKind.ReturnKeyword:
                    return "return";
                default:
                    return null;
            }
        }
    }

}
