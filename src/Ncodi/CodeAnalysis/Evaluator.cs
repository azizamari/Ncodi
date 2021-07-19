using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ncodi.CodeAnalysis.Binding;
using Ncodi.CodeAnalysis.Symbols;

namespace Ncodi.CodeAnalysis
{

    internal class Evaluator
    {
        public List<string> _outputLines;
        private readonly BoundProgram _program;
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();
        private readonly Dictionary<VariableSymbol, object> _globals;
        public readonly Stack<Dictionary<VariableSymbol, object>> _locals = new Stack<Dictionary<VariableSymbol, object>>();
        private Random _random;

        private object _lastValue;
        private bool _useConsole=true;
        private Func<Task<string>> _getInput;
        private Action<string> _sendOutput;
        private CancellationToken _token;

        public Evaluator(BoundProgram program, Dictionary<VariableSymbol, object> variables)
        {
            _program = program;
            _globals = variables;
            _locals.Push(new Dictionary<VariableSymbol, object>());
            _outputLines = new List<string>();
        }
        public ImmutableArray<Diagnostic> Diagnostics => _diagnostics.ToImmutableArray();
        public object Evaluate(bool useConsole=true, Func<Task<string>> GetInput=null, Action<string> send = null, CancellationToken token = default)
        {
            _token = token;
            _useConsole = useConsole;
            _getInput = GetInput;
            _sendOutput = send;
            return EvaluateStatement(_program.Statement);
        }

        private object EvaluateStatement(BoundBlockStatement body)
        {
            var labelToIndex = new Dictionary<BoundLabel, int>();

            for (var i = 0; i < body.Statements.Length; i++)
            {
                if (body.Statements[i] is BoundLabelStatement l)
                    labelToIndex.Add(l.Label, i + 1);
            }

            var index = 0;
            while (index < body.Statements.Length&& !_token.IsCancellationRequested)
            {
                var s = body.Statements[index];

                switch (s.Kind)
                {
                    case BoundNodeKind.VariableDeclaration:
                        EvaluateVariableDeclaration((BoundVariableDeclaration)s);
                        index++;
                        break;
                    case BoundNodeKind.ExpressionStatement:
                        EvaluateExpressionStatement((BoundExpressionStatement)s);
                        index++;
                        break;
                    case BoundNodeKind.GotoStatement:
                        var gs = (BoundGotoStatement)s;
                        index = labelToIndex[gs.Label];
                        break;
                    case BoundNodeKind.ConditionalGotoStatement:
                        var cgs = (BoundConditionalGotoStatement)s;
                        var condition = (bool)EvaluateExpression(cgs.Condition);
                        if (condition == cgs.JumpIfTrue)
                            index = labelToIndex[cgs.Label];
                        else
                            index++;
                        break;
                    case BoundNodeKind.LabelStatement:
                        index++;
                        break;
                    case BoundNodeKind.ReturnStatement:
                        var rs = (BoundReturnStatement)s;
                        _lastValue = rs.Expression == null ? null : EvaluateExpression(rs.Expression);
                        return _lastValue;
                    default:
                        throw new Exception($"Unexpected node {s.Kind}");
                }
            }

            return _lastValue;
        }

        private void EvaluateVariableDeclaration(BoundVariableDeclaration node)
        {
            var value = EvaluateExpression(node.Initializer);
            _lastValue = value;
            Assign(node.Variable, value);
        }

        private void EvaluateExpressionStatement(BoundExpressionStatement node)
        {
            _lastValue = EvaluateExpression(node.Expression);
        }

        private object EvaluateExpression(BoundExpression node)
        {
            //binary
            //number
            switch (node.Kind)
            {
                case BoundNodeKind.LiteralExpression:
                    return EvaluateLiteralExpression((BoundLiteralExpression)node);
                case BoundNodeKind.VariableExpression:
                    return EvaluateVariableExpression((BoundVariableExpression)node);
                case BoundNodeKind.AssignmentExpression:
                    return EvaluateAssignmentExpression((BoundAssignmentExpression)node);
                case BoundNodeKind.UnaryExpression:
                    return EvaluateUnaryExpression((BoundUnaryExpression)node);
                case BoundNodeKind.BinaryExpression:
                    return EvaluateBinaryExpression((BoundBinaryExpression)node);
                case BoundNodeKind.CallExpression:
                    return EvaluateCallExpression((BoundCallExpression)node);
                case BoundNodeKind.ConversionExpression:
                    return EvaluateConversionExpression((BoundConversionExpression)node);
                case BoundNodeKind.StringIndexExpression:
                    return EvaluateStringIndexExpression((BoundIndexExpression)node);
                default:
                    throw new Exception($"Unexpected node {node.Kind}");
            }
        }

        private object EvaluateStringIndexExpression(BoundIndexExpression node)
        {
            var text = EvaluateExpression(node.BoundExpression).ToString();
            var index = EvaluateExpression(node.IndexExpression);
            try
            {
                if(!(index is int))
                {
                    throw new Exception();
                }
                var i=Convert.ToInt32(index);
                if (i < -text.Length || i >= text.Length)
                {
                    _diagnostics.ReportIndexOutOfBounds(node.Location, text.Length, i);
                    return new BoundErrorExpression();
                }
                if (i < 0)
                    return Convert.ToString(text[^(-i)]);
                return Convert.ToString(text[i]);
            }
            catch (Exception)
            {
                _diagnostics.ReportIndexIsNotInt(node.Location,index);
                return new BoundErrorExpression();
            }
        }

        private object EvaluateConversionExpression(BoundConversionExpression node)
        {
            var value = EvaluateExpression(node.Expression);
            if (node.Type == TypeSymbol.Bool)
                return Convert.ToBoolean(value);
            else if (node.Type == TypeSymbol.Int)
                try
                {
                    return Convert.ToInt32(value);
                }
                catch (Exception)
                {
                    _diagnostics.ReportCannotConvertToInt(node.Location, value);
                    return new BoundErrorExpression();
                }
            else if (node.Type == TypeSymbol.String)
                return Convert.ToString(value);
            else if (node.Type == TypeSymbol.Decimal)
            {
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch (Exception)
                {
                    _diagnostics.ReportCannotConvertToDecimal(node.Location, value);
                    return new BoundErrorExpression();
                }
            }
            else
                throw new Exception($"Unexpected type {node.Type}");
        }

        private object EvaluateBinaryExpression(BoundBinaryExpression b)
        {
            var left = EvaluateExpression(b.Left);
            var right = EvaluateExpression(b.Right);
            switch (b.Op.Kind)
            {
                case BoundBinaryOperatorKind.Addition:
                    {
                        if (b.Type == TypeSymbol.String)
                            return (string)left + (string)right;
                        var res= Convert.ToDecimal(left) + Convert.ToDecimal(right);
                        if (b.Type == TypeSymbol.Decimal)
                            return res;
                        return (int)res;
                    }
                case BoundBinaryOperatorKind.Substraction:
                    {
                        var res = Convert.ToDecimal(left) - Convert.ToDecimal(right);
                        if (b.Type == TypeSymbol.Decimal)
                            return res;
                        return (int)res;
                    }
                case BoundBinaryOperatorKind.Multiplication:
                    {
                        var res = Convert.ToDecimal(left) * Convert.ToDecimal(right);
                        if (b.Type == TypeSymbol.Decimal)
                            return res;
                        return (int)res;
                    }
                case BoundBinaryOperatorKind.Power:
                    {
                        var res = Convert.ToDecimal(Math.Pow(Convert.ToDouble(left), Convert.ToDouble(right)));
                        if (b.Type == TypeSymbol.Decimal)
                            return res;
                        return (int)res;
                    }
                case BoundBinaryOperatorKind.EuclidianDivision:
                    return (int)left / (int)right;
                case BoundBinaryOperatorKind.DecimalDivision:
                    return Convert.ToDecimal(left) / Convert.ToDecimal(right);
                case BoundBinaryOperatorKind.DivisionRemainder:
                    return (int)left % (int)right;

                case BoundBinaryOperatorKind.BitwiseAnd:
                    if(b.Type==TypeSymbol.Int)
                        return (int)left & (int)right;
                    else
                        return (bool)left & (bool)right;
                case BoundBinaryOperatorKind.BitwiseOr:
                    if (b.Type == TypeSymbol.Int)
                        return (int)left | (int)right;
                    else
                        return (bool)left | (bool)right;
                case BoundBinaryOperatorKind.BitwiseXor:
                    if (b.Type == TypeSymbol.Int)
                        return (int)left ^ (int)right;
                    else
                        return (bool)left ^ (bool)right;

                case BoundBinaryOperatorKind.LogicalAnd:
                    return (bool)left && (bool)right;
                case BoundBinaryOperatorKind.LogicalOr:
                    return (bool)left || (bool)right;
                case BoundBinaryOperatorKind.LessThan:
                    if (b.Type == TypeSymbol.String)
                        return string.Compare(Convert.ToString(left) , Convert.ToString(right), StringComparison.Ordinal)<0;
                    return Convert.ToDecimal(left)< Convert.ToDecimal(right);
                case BoundBinaryOperatorKind.LessThanOrEquals:
                    if (b.Type == TypeSymbol.String)
                        return string.Compare(Convert.ToString(left), Convert.ToString(right), StringComparison.Ordinal) <= 0;
                    return Convert.ToDecimal(left) <= Convert.ToDecimal(right);
                case BoundBinaryOperatorKind.GreaterThan:
                    if (b.Type == TypeSymbol.String)
                        return string.Compare(Convert.ToString(left), Convert.ToString(right), StringComparison.Ordinal)>0;
                    return Convert.ToDecimal(left) > Convert.ToDecimal(right);
                case BoundBinaryOperatorKind.GreaterThanOrEquals:
                    if (b.Type == TypeSymbol.String)
                        return string.Compare(Convert.ToString(left), Convert.ToString(right), StringComparison.Ordinal) >= 0;
                    return Convert.ToDecimal(left) >= Convert.ToDecimal(right);
                case BoundBinaryOperatorKind.Equals:
                    {
                        if (b.Left.Type == TypeSymbol.Decimal ^ b.Right.Type == TypeSymbol.Decimal)
                        {
                            return Equals(Convert.ToDecimal(left), Convert.ToDecimal(right));
                        }
                        return Equals(left, right);
                    }
                case BoundBinaryOperatorKind.NotEquals:
                    {
                        if (b.Left.Type == TypeSymbol.Decimal ^ b.Right.Type == TypeSymbol.Decimal)
                        {
                            return !Equals(Convert.ToDecimal(left), Convert.ToDecimal(right));
                        }
                        return !Equals(left, right);
                    }
                default:
                    throw new Exception($"Unexpected Operator {b.Op.Kind}");
            }
        }

        private object EvaluateUnaryExpression(BoundUnaryExpression u)
        {
            var operand = EvaluateExpression(u.Operand);
            switch (u.Op.Kind)
            {
                case BoundUnaryOperatorKind.Identity:
                    if (u.Type == TypeSymbol.Int)
                        return (int)operand;
                    return (decimal)operand;
                case BoundUnaryOperatorKind.Negation:
                    if(u.Type==TypeSymbol.Int)
                        return -(int)operand;
                    return -(decimal)operand;
                case BoundUnaryOperatorKind.LogicalNegation:
                    return !(bool)operand;
                case BoundUnaryOperatorKind.OnesComplement:
                    return ~(int)operand;
                default:
                    throw new Exception($"Unexpected Operator {u.Op.Kind}");
            }
        }

        private object EvaluateAssignmentExpression(BoundAssignmentExpression a)
        {
            var value = EvaluateExpression(a.Expression);
            Assign(a.Variable, value);
            return value;
        }

        private object EvaluateVariableExpression(BoundVariableExpression v)
        {
            if (v.Variable.Kind == SymbolKind.GlobalVariable)
            {
                return _globals[v.Variable];
            }
            else
            {
                var locals = _locals.Peek();
                return locals[v.Variable];
            }
        }

        private static object EvaluateLiteralExpression(BoundLiteralExpression n)
        {
            return n.Value;
        }

        private object EvaluateCallExpression(BoundCallExpression node)
        {
            if (node.Function == BuiltInFunctions.Input)
            {
                if (!_useConsole)
                {
                    var x=_getInput();
                    Task.WaitAll(x);
                    return x.Result;
                }
                else
                    return Console.ReadLine();
            }
            else if (node.Function == BuiltInFunctions.Print)
            {
                try
                {
                    var result="";
                    var message = EvaluateExpression(node.Arguments[0]);
                    if (node.Arguments[0].Type == TypeSymbol.String)
                        result = (string)message;
                    if (node.Arguments[0].Type == TypeSymbol.Bool)
                    {
                        var val = (bool)message;
                        if (val) result = "s7i7";
                        else result = "ghalet";
                    }
                    else
                        result = $"{message}";
                    if (_useConsole)
                        Console.WriteLine(result);
                    //_outputLines.Add(result);
                    _sendOutput(result);
                    return null;
                }
                catch
                {
                    return null;
                }
            }
            else if (node.Function == BuiltInFunctions.Chr)
            {
                var number = Convert.ToInt32(EvaluateExpression(node.Arguments[0]));
                if (number > 255 || number < 0)
                {
                    _diagnostics.ReportAsciiBounds(node.Location, number);
                    return new BoundErrorExpression();
                }
                return Convert.ToChar(number).ToString();
            }
            else if (node.Function == BuiltInFunctions.Ord)
            {
                var character = (string)EvaluateExpression(node.Arguments[0]);
                if (character.Length != 1)
                {
                    _diagnostics.ReportIsNotChar(node.Location, character);
                    return new BoundErrorExpression();
                }
                return (byte)character[0];
            }
            else if (node.Function == BuiltInFunctions.Len)
            {
                var message = (string)EvaluateExpression(node.Arguments[0]);
                return message.Length;
            }
            else if (node.Function == BuiltInFunctions.Random)
            {
                var max = (int)EvaluateExpression(node.Arguments[0]);
                if (_random == null)
                    _random = new Random();
                return _random.Next(max);
            }
            else if (node.Function == BuiltInFunctions.Sqrt)
            {
                var number = (int)EvaluateExpression(node.Arguments[0]);
                return Math.Sqrt(number);
            }
            else
            {
                var locals = new Dictionary<VariableSymbol, object>();
                for (int i = 0; i < node.Arguments.Length; i++)
                {
                    var parameter = node.Function.Parameters[i];
                    var value = EvaluateExpression(node.Arguments[i]);
                    locals.Add(parameter, value);
                }
                _locals.Push(locals);
                var statement = _program.Functions[node.Function];
                var result = EvaluateStatement(statement);

                _locals.Pop();
                 
                return result;
            }
        }
        private void Assign(VariableSymbol variable, object value)
        {
            if (variable.Kind == SymbolKind.GlobalVariable)
            {
                _globals[variable] = value;
            }
            else
            {
                var locals = _locals.Peek();
                locals[variable] = value;
            }
        }
    }
}
