using System.Collections.Generic;

namespace Pital.CodeAnalysis.Syntax
{
   internal sealed class Lexer
    {
        private readonly string _text;
        private int _position;
        private DiagnosticBag _diagnostics = new DiagnosticBag();

        public Lexer(string text)
        {
            _text = text;
        }

        public DiagnosticBag Diagnostics => _diagnostics;
        private char current => Peek(0);
        private char lookAhead => Peek(1);

        private char Peek(int offset)
        {
            var index = _position + offset;
            if (index >= _text.Length)
                return '\0';
            return _text[index];
        }

        private void Next()
        {
            _position++;
        }

        public SyntaxToken Lex()
        {
            //numeric

            //operators
            //<ws>

            if (_position >= _text.Length)
            {
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);
            }

            var start = _position;
            if (char.IsDigit(current))
            {
                while (char.IsDigit(current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                if (!int.TryParse(text, out var value))
                {
                    _diagnostics.ReportInvalidNumber(new TextSpan(start,length),_text,typeof(int));
                }
                return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
            }

            if (char.IsWhiteSpace(current))
            {
                while (char.IsWhiteSpace(current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                return new SyntaxToken(SyntaxKind.WhiteSpaceToken, start, text, null);
            }

            //true & false
            if (char.IsLetter(current))
            {
                while (char.IsLetter(current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                var kind = SyntaxFacts.GetKeywordKind(text);
                return new SyntaxToken(kind, start, text, null);
            }


            switch (current)
            {
                case '+':
                    return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null);
                case '-':
                    return new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null);
                case '*':
                    return new SyntaxToken(SyntaxKind.StarToken, _position++, "*", null);
                case '/':
                    return new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null);
                case '(':
                    return new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++, "(", null);
                case ')':
                    return new SyntaxToken(SyntaxKind.ClosedParenthesisToken, _position++, ")", null);
                case '&':
                    return new SyntaxToken(SyntaxKind.AmpersandToken, _position++, "&", null);
                case '|':
                    return new SyntaxToken(SyntaxKind.PipeToken, _position ++, "|", null);
                case '=':
                    if(lookAhead == '=')
                    {
                        _position += 2;
                        return new SyntaxToken(SyntaxKind.EqualsEqualsToken, start, "==", null);
                    }
                        
                    break;
                case '!':
                    if (lookAhead == '=')
                    {
                        _position += 2;
                        return new SyntaxToken(SyntaxKind.BangEqualsToken, start, "!=", null);
                    }
                    else
                        return new SyntaxToken(SyntaxKind.BangToken, _position++, "!", null);
            }

            _diagnostics.ReportBadCharacter(_position,current);
            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }
    }
}
