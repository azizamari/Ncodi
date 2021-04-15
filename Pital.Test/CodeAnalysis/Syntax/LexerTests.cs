using Pital.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Pital.Test.CodeAnalysis.Syntax
{
    public class LexerTests
    {
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
            Assert.Equal(tokens[0].Kind, kind1);
            Assert.Equal(tokens[1].Kind, seperatorKind);
            Assert.Equal(tokens[2].Kind, kind2);
            Assert.Equal(tokens[0].Text, text1);
            Assert.Equal(tokens[1].Text, seperatorText);
            Assert.Equal(tokens[2].Text, text2);
        }

        public static IEnumerable<object[]> GetTokensData()
        {
            foreach(var t in GetTokens().Concat(GetSeperators()))
            {
                yield return new object[] { t.kind, t.text };
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
            foreach (var t in GetTokenPairsWithSeperators())
            {
                yield return new object[] { t.kind1, t.text1, t.seperatorKind, t.seperatorText, t.kind2, t.text2 };
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
            if (isKeyword1 && kind2 == SyntaxKind.IdentifierToken || isKeyword2 && kind1 == SyntaxKind.IdentifierToken)
                return true;
            if (kind1 == SyntaxKind.NumberToken && kind2 == SyntaxKind.NumberToken)
                return true;
            if (kind1 == SyntaxKind.BangToken && kind2 == SyntaxKind.EqualsToken)
                return true;
            if (kind1 == SyntaxKind.EqualsToken && kind2 == SyntaxKind.EqualsToken)
                return true;
            if (kind1 == SyntaxKind.EqualsToken && kind2 == SyntaxKind.EqualsEqualsToken)
                return true;
            if (kind1 == SyntaxKind.BangToken && kind2 == SyntaxKind.EqualsEqualsToken)
                return true;
            //if (kind1 == SyntaxKind.WhiteSpaceToken && kind2 == kind1)
            //    return true;
            //TODO: more cases

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
