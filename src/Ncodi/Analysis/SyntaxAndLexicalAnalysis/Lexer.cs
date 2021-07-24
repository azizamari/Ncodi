using Ncodi.CodeAnalysis.Symbols;
using Ncodi.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Text;

namespace Ncodi.CodeAnalysis.Syntax
{
   internal sealed class Lexer
    {
        private readonly ErrorBag _diagnostics = new ErrorBag();
        private readonly SourceText _text;
        private readonly SyntaxTree _syntaxTree;
        private int _position;

        private int _start;
        private SyntaxKind _kind;
        private object _value;
          
        public Lexer(SyntaxTree syntaxTree)
        {
            _text = syntaxTree.Text;
            _syntaxTree = syntaxTree;
        }

        public ErrorBag Diagnostics => _diagnostics;
        private char Current => Peek(0);
        private char LookAhead => Peek(1);

        private char Peek(int offset)
        {
            var index = _position + offset;
            if (index >= _text.Length)
                return '\0';
            return _text[index];
        }

        public SyntaxToken Lex()
        {
            //numeric

            //operators
            //<ws>

            _start = _position;
            _kind = SyntaxKind.BadToken;
            _value = null;


            switch (Current)
            {
                case '\0':
                    _kind=SyntaxKind.EndOfFileToken;
                    break;
                case '+':
                    _kind = SyntaxKind.PlusToken;
                    _position++;
                    break;
                case '-':
                    _kind = SyntaxKind.MinusToken;
                    _position++;
                    break;
                case '%':
                    _kind = SyntaxKind.ModuloToken;
                    _position++;
                    break;
                case '(':
                    _kind = SyntaxKind.OpenParenthesisToken;
                    _position++;
                    break;
                case ')':
                    _kind = SyntaxKind.ClosedParenthesisToken;
                    _position++;
                    break;
                case '[':
                    _kind = SyntaxKind.OpenBracketToken;
                    _position++;
                    break;
                case ']':
                    _kind = SyntaxKind.ClosedBracketToken;
                    _position++;
                    break;
                case '{':
                    _kind = SyntaxKind.OpenBraceToken;
                    _position++;
                    break;
                case '}':
                    _kind = SyntaxKind.ClosedBraceToken;
                    _position++;
                    break;
                case '~':
                    _kind = SyntaxKind.TildeToken;
                    _position++;
                    break;
                case '^':
                    _kind = SyntaxKind.HatToken;
                    _position++;
                    break;
                case ',':
                    _kind = SyntaxKind.CommaToken;
                    _position++;
                    break;
                case ':':
                    _kind = SyntaxKind.ColonToken;
                    _position++;
                    break;
                case '/':
                    _position++;
                    if (Current != '/')
                    {
                        _kind = SyntaxKind.SlashToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.SlashSlashToken;
                        _position++;
                    }
                    break;
                case '*':
                    _position++;
                    if (Current != '*')
                    {
                        _kind = SyntaxKind.StarToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.StarStarToken;
                        _position++;
                    }
                    break;
                case '&':
                    _position++;
                    if (Current != '&')
                    {
                        _kind = SyntaxKind.AmpersandToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.AmpersandAmpersandToken;
                        _position++;
                    }
                    break;
                case '|':
                    _position++;
                    if (Current != '|')
                    {
                        _kind = SyntaxKind.PipeToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.PipePipeToken;
                        _position++;
                    }
                    break;
                case '=':
                    _position++;
                    if (Current != '=')
                    {
                        _kind = SyntaxKind.EqualsToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.EqualsEqualsToken;
                        _position++;
                    }
                    break;
                case '!':
                    _position++;
                    if (Current != '=')
                    {
                        _kind = SyntaxKind.BangToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.BangEqualsToken;
                        _position++;
                    }
                    break;
                case '>':
                    _position++;
                    if (Current != '=')
                    {
                        _kind = SyntaxKind.GreaterToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.GreaterOrEqualsToken;
                        _position++;
                    }
                    break;
                case '<':
                    _position++;
                    if (Current != '=')
                    {
                        _kind = SyntaxKind.LessToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.LessOrEqualsToken;
                        _position++;
                    }
                    break;
                case '"':
                    ReadString();
                    break;
                case '0': case '1':case '2':case '3':case '4':
                case '5': case '6':case '7':case '8':case '9':
                    ReadNumberToken();
                    break;
                case ' ':
                case '\t':
                case '\r':
                case '\n':
                    ReadWhiteSpace();
                    break;
                default:
                    if (char.IsLetter(Current))
                    {
                        ReadIdentifierOrKeyword();

                    }
                    else if (char.IsWhiteSpace(Current))
                    {
                        ReadWhiteSpace();
                    }
                    else
                    {
                        var span = new TextSpan(_position, 1);
                        var location = new TextLocation(_text, span);
                        _diagnostics.ReportBadCharacter(location,Current);
                        _position += 1;
                    }
                    break;

            }
            var length = _position - _start;
            var text = _text.ToString(_start,length);
            if (text == null)
            {
                text = _text.ToString(_start, length);
            }
            return new SyntaxToken(_syntaxTree, _kind, _start, text, _value);
        }

        private void ReadString()
        {
            //skip first quote "
            _position++;
            var stringBuilder = new StringBuilder();
            var done = false;

            while (!done)
            {
                switch (Current)
                {
                    case '\0':
                    case '\r':
                    case '\n':
                        var span = new TextSpan(_start,1);
                        var location = new TextLocation(_text, span);
                        _diagnostics.ReportUnterminatedString(location);
                        done = true;
                        break;
                    case '"':
                        if (LookAhead == '"')
                        {
                            stringBuilder.Append(Current);
                            _position+=2;
                        }
                        else
                        {
                            _position++;
                            done = true;
                        }
                        break;
                    default:
                        stringBuilder.Append(Current);
                        _position++;
                        break;
                }
            }
            _kind = SyntaxKind.StringToken;
            _value = stringBuilder.ToString();
        }

        private void ReadWhiteSpace()
        {
            while (char.IsWhiteSpace(Current))
                _position++;

            _kind = SyntaxKind.WhiteSpaceToken;
        }

        private void ReadNumberToken()
        {
            var countDots=0;
            var curr = Current;
            while (char.IsDigit(curr) || curr == '.')
            {
                if (curr == '.')
                    countDots++;
                _position++;
                curr = Current;
            }

            var length = _position - _start;
            var text = _text.ToString(_start, length);
            if (countDots > 1)
            {
                var span = new TextSpan(_start, length);
                var location = new TextLocation(_text, span);

                _diagnostics.ReportInvalidNumber(location, text, TypeSymbol.Decimal);
            }
            if (countDots == 1)
            {
                if (!decimal.TryParse(text, out var value))
                {
                    var span = new TextSpan(_start, length);
                    var location = new TextLocation(_text, span);

                    _diagnostics.ReportInvalidNumber(location, text, TypeSymbol.Decimal);
                }
                _value = value;
                _kind = SyntaxKind.DecimalToken;
            }
            else
            {
                if (!int.TryParse(text, out var value))
                {
                    var span = new TextSpan(_start, length);
                    var location = new TextLocation(_text, span);

                    _diagnostics.ReportInvalidNumber(location, text, TypeSymbol.Int);
                }
                _value = value;
                _kind = SyntaxKind.NumberToken;
            }
        }

        private void ReadIdentifierOrKeyword()
        {
            while (char.IsLetter(Current) ||( char.IsNumber(Current)&&_position>_start))
                _position++;

            var length = _position - _start;
            var text = _text.ToString(_start, length);
            _kind = SyntaxFacts.GetKeywordKind(text);
        }

    }
}
