using Ncodi.CodeAnalysis.Syntax;
using Ncodi.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Ncodi.Test.CodeAnalysis.Syntax
{
    public class LexerTests
    {

        [Fact]
        public void Lexer_Lexes_UnterminatedString()
        {
            var  text = "\"aziz";
            var tokens = SyntaxTree.ParseTokens(text, out var diagnostics);

            var token = Assert.Single(tokens);
            Assert.Equal(SyntaxKind.StringToken, token.Kind);
            Assert.Equal(text, token.Text);

            var diagnotic = Assert.Single(diagnostics);
            Assert.Equal(new TextSpan(0,1), diagnotic.Location.Span);
            Assert.Equal("Unterminated string literal", diagnotic.Message);
        }

        [Fact]
        public void Lexer_Covers_AllTokens()
        {
            var tokenKinds = Enum.GetValues(typeof(SyntaxKind))
                .Cast<SyntaxKind>()
                .Where(k => k.ToString().EndsWith("Keyword") || k.ToString().EndsWith("Token"));

            var testedTokenKinds = GetTokens().Concat(GetSeperators()).Select(t=>t.kind);
            var untestedTokenKinds = new SortedSet<SyntaxKind>(tokenKinds);
            untestedTokenKinds.Remove(SyntaxKind.BadToken);
            untestedTokenKinds.Remove(SyntaxKind.EndOfFileToken);
            untestedTokenKinds.ExceptWith(testedTokenKinds);
            Assert.Empty(untestedTokenKinds);
        }

        [Theory]
        [MemberData(nameof(GetTokensData))]
        public void Lexer_Lexes_Token(SyntaxKind kind, string text) 
        {
            var tokens = SyntaxTree.ParseTokens(text);
            var token = Assert.Single(tokens);
            Assert.Equal(kind, token.Kind);
            Assert.Equal(text, token.Text);
        }

        [Theory]
        [MemberData(nameof(GetTokenPairsData))]
        public void Lexer_Lexes_Token_Pairs(SyntaxKind kind1, string text1, SyntaxKind kind2, string text2)
        {
            var text = text1 + text2;
            var tokens = SyntaxTree.ParseTokens(text).ToArray();
            Assert.Equal(2,tokens.Length);
            Assert.Equal(tokens[0].Kind, kind1);
            Assert.Equal(tokens[1].Kind, kind2);
            Assert.Equal(tokens[0].Text, text1);
            Assert.Equal(tokens[1].Text, text2);
        }

        [Theory]
        [MemberData(nameof(GetTokenPairsWithSeperatorsData))]
        public void Lexer_Lexes_Token_Pairs_With_Seperators(SyntaxKind kind1, string text1,SyntaxKind seperatorKind, string seperatorText, SyntaxKind kind2, string text2)
        {
            var text = text1 +seperatorText+ text2;
            var tokens = SyntaxTree.ParseTokens(text).ToArray();
            Assert.Equal(3, tokens.Length);
            Assert.Equal(kind1, tokens[0].Kind);
            Assert.Equal(seperatorKind, tokens[1].Kind);
            Assert.Equal(kind2,tokens[2].Kind);
            Assert.Equal(text1, tokens[0].Text );
            Assert.Equal(seperatorText, tokens[1].Text);
            Assert.Equal(text2, tokens[2].Text);
        }

        public static IEnumerable<object[]> GetTokensData()
        {
            foreach(var (kind, text) in GetTokens().Concat(GetSeperators()))
            {
                yield return new object[] { kind, text };
            }
        }
        public static IEnumerable<object[]> GetTokenPairsData()
        {
            foreach (var t in GetTokenPairs())
            {
                yield return new object[] { t.kind1, t.text1, t.kind2, t.text2 };
            }
        }
        public static IEnumerable<object[]> GetTokenPairsWithSeperatorsData()
        {
            foreach (var (kind1, text1, seperatorKind, seperatorText, kind2, text2) in GetTokenPairsWithSeperators())
            {
                yield return new object[] { kind1, text1, seperatorKind, seperatorText, kind2, text2 };
            }
        }

        private static IEnumerable<(SyntaxKind kind,string text)> GetTokens()
        {
            var fixedTokens = Enum.GetValues(typeof(SyntaxKind))
                .Cast<SyntaxKind>()
                .Select(k => (kind: k, text: SyntaxFacts.GetText(k)))
                .Where(t=>t.text!=null);
            var dynamicTokens = new[]
            {
                (SyntaxKind.IdentifierToken,"a"),
                (SyntaxKind.IdentifierToken,"abc"),
                (SyntaxKind.NumberToken,"3526"),
                (SyntaxKind.NumberToken,"1"),
                (SyntaxKind.StringToken,"\"Aziz\""),
                (SyntaxKind.StringToken,"\"A\"\"ziz\""),
            };
            return fixedTokens.Concat(dynamicTokens);
        }

        private static IEnumerable<(SyntaxKind kind, string text)> GetSeperators()
        {
            return new[]
            {
                (SyntaxKind.WhiteSpaceToken,"  "),
                (SyntaxKind.WhiteSpaceToken,"   "),
                (SyntaxKind.WhiteSpaceToken,"\r"),
                (SyntaxKind.WhiteSpaceToken,"\n"),
                (SyntaxKind.WhiteSpaceToken,"\r\n"),
            };
        }

        public static bool RequiresSeparator(SyntaxKind kind1,SyntaxKind kind2)
        {
            var isKeyword1 = kind1.ToString().EndsWith("Keyword");
            var isKeyword2 = kind2.ToString().EndsWith("Keyword");

            if (kind1 == SyntaxKind.IdentifierToken && kind2 == SyntaxKind.IdentifierToken)
                return true;

            if (isKeyword1 && isKeyword2)
                return true;

            if (isKeyword1 && kind2 == SyntaxKind.IdentifierToken)
                return true;

            if (kind1 == SyntaxKind.IdentifierToken && isKeyword2)
                return true;

            if (kind1 == SyntaxKind.NumberToken && kind2 == SyntaxKind.NumberToken)
                return true;

            if (kind1 == SyntaxKind.BangToken && kind2 == SyntaxKind.EqualsToken)
                return true;

            if (kind1 == SyntaxKind.BangToken && kind2 == SyntaxKind.EqualsEqualsToken)
                return true;

            if (kind1 == SyntaxKind.EqualsToken && kind2 == SyntaxKind.EqualsToken)
                return true;

            if (kind1 == SyntaxKind.EqualsToken && kind2 == SyntaxKind.EqualsEqualsToken)
                return true;
            if (kind1 == SyntaxKind.LessToken && kind2 == SyntaxKind.EqualsToken)
                return true;
            if (kind1 == SyntaxKind.LessToken && kind2 == SyntaxKind.EqualsEqualsToken)
                return true;
            if (kind1 == SyntaxKind.GreaterToken && kind2 == SyntaxKind.EqualsToken)
                return true;
            if (kind1 == SyntaxKind.GreaterToken && kind2 == SyntaxKind.EqualsEqualsToken)
                return true;
            if (kind1 == SyntaxKind.AmpersandToken && kind2 == SyntaxKind.AmpersandAmpersandToken)
                return true;

            if (kind1 == SyntaxKind.AmpersandToken && kind2 == SyntaxKind.AmpersandToken)
                return true;
            if (kind1 == SyntaxKind.PipeToken && kind2 == SyntaxKind.PipePipeToken)
                return true;

            if (kind1 == SyntaxKind.PipeToken && kind2 == SyntaxKind.PipeToken)
                return true;
            if (kind1 == SyntaxKind.StringToken && kind2 == SyntaxKind.StringToken)
                return true;
            if (kind1 == SyntaxKind.IdentifierToken && kind2 == SyntaxKind.NumberToken)
                return true;
            if (kind1.ToString().EndsWith("Keyword") && kind2 == SyntaxKind.NumberToken)
                return true;

            return false;
        }

        private static IEnumerable<(SyntaxKind kind1,string text1, SyntaxKind kind2, string text2)> GetTokenPairs()
        {
            foreach(var t1 in GetTokens())
            {
                foreach(var t2 in GetTokens())
                {
                    if (!RequiresSeparator(t1.kind, t2.kind))
                    {
                        yield return (t1.kind, t1.text, t2.kind, t2.text);
                    }
                }
            }
        }

        private static IEnumerable<(SyntaxKind kind1, string text1,SyntaxKind seperatorKind,string seperatorText, SyntaxKind kind2, string text2)> GetTokenPairsWithSeperators()
        {
            foreach (var t1 in GetTokens())
            {
                foreach (var t2 in GetTokens())
                {
                    if (RequiresSeparator(t1.kind, t2.kind))
                    {
                        foreach(var s in GetSeperators())
                            yield return (t1.kind, t1.text,s.kind,s.text, t2.kind, t2.text);
                    }
                }
            }
        }


    }
}
