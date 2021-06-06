using Ncodi.CodeAnalysis.Symbols;
using Ncodi.CodeAnalysis.Syntax;
using Ncodi.CodeAnalysis.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ncodi.CodeAnalysis
{
    internal sealed class DiagnosticBag : IEnumerable<Diagnostic>
    {
        private readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();

        public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void Report(TextLocation location,string message)
        {
            var diagnostic = new Diagnostic(location, message);
            _diagnostics.Add(diagnostic);
        }

        public void ReportInvalidNumber(TextLocation span, string text, TypeSymbol type)
        {
            //var message = $"The number {text} isn't a valid {type}";
            var message = $"{text} mahouch {type} s7i7";
            Report(span, message);
        }

        public void AddRange(DiagnosticBag diagnostics)
        {
            _diagnostics.AddRange(diagnostics._diagnostics);
        }
        public void ReportBadCharacter(TextLocation location, char character)
        {
            //var message = $"Bad character input: '{character}'";
            var message = $"L 7arf '{character}' mane3rfech";
            Report(location, message);
        }

        public  void ReportUnexpectedToken(TextLocation location, SyntaxKind actualKind, SyntaxKind expectedKind)
        {
            //var message = $"Unexpected token <{actualKind}>, expected <{expectedKind}>";
            var message = $"Twa9e3t <{expectedKind}> ama lgit <{actualKind}>";
            Report(location, message);
        }

        public void ReportUndefinedUnaryOperator(TextLocation location, string operatorText, TypeSymbol operandText)
        {
            var message = $"Unary operator '{operatorText}' is not defined for type '{operandText}'";
            Report(location, message);
        }

        public void ReportUndefinedBinaryOperator(TextLocation location, string operatorText, TypeSymbol leftType, TypeSymbol rightType)
        {
            var message = $"Binary operator '{operatorText}' is not defined for types '{leftType}' and '{rightType}'";
            Report(location, message);
        }

        public void ReportUndefinedName(TextLocation location, string name)
        {
            //var message = $"Variable '{name}' doesn't exist";
            var message = $"Variable '{name}' Mahouch mawjoud";
            Report(location, message);
        }

        public void ReportParameterAlreadyDeclared(TextLocation location, string parameterName)
        {
            //var message = $"Parameter named '{parameterName}' already exists";
            var message = $"Parameter '{parameterName}' mawjoud";
            Report(location, message);
        }

        public void ReportCannotConvert(TextLocation location, TypeSymbol type1, TypeSymbol type2)
        {
            //var messsage = $"Cannot convert type '{type1}' to '{type2}'";
            var messsage = $"Najemch n7wal type '{type1}' l '{type2}'";
            Report(location, messsage);
        }


        public void ReportCannotConvertImplicitly(TextLocation location, TypeSymbol type1, TypeSymbol type2)
        {
            //var message = $"Cannot implicitly convert type '{type1}' to '{type2}'. An Explicit convesion exists ( are you missing a cast?)";
            var message = $"Najemch n7wal type '{type1}' l '{type2}'";
            Report(location, message);
        }

        public void ReportAllPathsMustReturn(TextLocation location)
        {
            //var message = "Some code paths don't return a value";
            var message = "Mch lcode lkol iraje3 natija";
            Report(location, message);
        }

        public void ReportSymbolAlreadyDeclared(TextLocation location, string name)
        {
            //var message = $"Symbol '{name}' is already declared as a function or as a variable";
            var message = $"L esm '{name}' déja mawjoud ka fonction wela variable";
            Report(location, message);
        }

        public void ReportCannotAssign(TextLocation location, string name)
        {
            //var message = $"Variable '{name}' is read-only and cannot be assigned to";
            var message = $"L variable '{name}' const w ma tnajmch tbadla";
            Report(location, message);
        }

        public void ReportUnterminatedString(TextLocation location)
        {
            //var message = "Unterminated string literal";
            var message = "L kalma mahich maktoba b s7i7";
            Report(location, message);
        }
        public void ReportUndefinedFunction(TextLocation location, string name)
        {
            //var message = $"Function '{name}' doesn't exist";
            var message = $"L fonction '{name}' mahich mawjoda";
            Report(location, message);
        }

        public void ReportWrongArgumentCount(TextLocation location, string name, int expectedCount, int actualCount)
        {
            //var message = $"Function '{name}' requires {expectedCount} arguments but was given {actualCount}";
            var message = $"L fonction '{name}' test7a9 {expectedCount} argument ama enta 3titha {actualCount}";
            Report(location, message);
        }

        public void ReportWrongArgumentType(TextLocation location, string name, TypeSymbol expectedType, TypeSymbol actualType)
        {
            var message = $"Parameter '{name}' requires a value of type '{expectedType}' but was given a value of type '{actualType}'";
            Report(location, message);
        }

        public void ReportExpressionMustHaveValue(TextLocation location)
        {
            //var message = "Expression must have a value";
            var message = "L expression ma3endhech 9ima";
            Report(location, message);
        }

        public void ReportUndefinedType(TextLocation location, string name)
        {
            //var message = $"Type '{name}' doesn't exist";
            var message = $"L type '{name}' mahouch mawjoud";
            Report(location, message);
        }

        public void ReportInvalidBreakOrContinue(TextLocation location, string text)
        {
            //var message = $"Keyword '{text}' can't be used outside of functions";
            var message = $"Ma tnajemch testa3mel '{text}' l bara men fonction";
            Report(location, message);
        }

        public void ReportCannotConvertToInt(TextLocation location, object value)
        {
            //var messsage = $"Cannot convert \"{value}\" to 'Int'";
            var messsage = $"Ma ngedech n7tawel \"{value}\" l 'Int'";
            Report(location, messsage);
        }

        public void ReportIndexOutOfBounds(TextLocation location, int length, int i)
        {
            //var messsage = $"Index out of bounds string is of length {length} and the index is {i}";
            var messsage = $"L index {i} l bara mel 7oudoud [{-length}..0..{length-1}]";
            Report(location, messsage);
        }

        public void ReportIndexIsNotInt(TextLocation location, object index)
        {
            //var messsage = $"Index '{index}' is not an Int";
            var messsage = $"L index '{index}' mahouch type Int";
            Report(location, messsage);
        }

        public void ReportInvalidReturn(TextLocation location)
        {
            //var message = "The 'return' keyword can't be used outside of functions";
            var message = $"Ma tnajemch testa3ml {SyntaxKind.ReturnKeyword.ToString()} l bara men fonction";
            Report(location, message);
        }

        public void ReportMissingReturnExpression(TextLocation location, TypeSymbol returnType)
        {
            //var message = $"Expression of type '{returnType}' expected";
            var message = $"Twa9et expression type '{returnType}'";
            Report(location, message);
        }

        public void ReportCannotConvertToDecimal(TextLocation location, object value)
        {
            //var messsage = $"Cannot convert \"{value.ToString()}\" to 'Decimal'";
            var messsage = $"Ma najemch n7awel \"{value.ToString()}\" l 'Decimal'";
            Report(location, messsage);
        }

        public void ReportInvalidReturnExpression(TextLocation location, string functionName)
        {
            //var message = $"The function '{functionName}' does not return a value the 'return' keyword can't be followed by an expression";
            var message = $"L fonction '{functionName}' ma traje3 7ata natijfa";
            Report(location, message);
        }

        public void ReportAsciiBounds(TextLocation location, int number)
        {
            var message = $"Cannot find an ASCII match for '{number}' since it's not in the interval [0..255]";
            Report(location, message);
        }

        public void ReportIsNotChar(TextLocation location, string character)
        {
            var message = $"Cannot convert '{character}' to ASCII int since it's not a single character";
            Report(location, message);
        }

        public void ReportIndexingNotAllowedForType(TextLocation location, TypeSymbol type)
        {
            var message = $"Indexing [index] not allowed for type '{type}' only for string";
            Report(location, message);
        }
    }
}
