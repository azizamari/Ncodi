using Ncodi.CodeAnalysis.Syntax;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using Ncodi.CodeAnalysis.Symbols;
using Ncodi.CodeAnalysis.Text;
using Ncodi.CodeAnalysis.Lowering;

namespace Ncodi.CodeAnalysis.Binding
{
    internal sealed class Binder
    {
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();
        private readonly FunctionSymbol _function;
        private Stack<(BoundLabel BreakLabel, BoundLabel ContinueLabel)> _loopStack = new Stack<(BoundLabel BreakLabel, BoundLabel ContinueLabel)>();
        private int _labelCounter;
        private BoundScope _scope;

        public Binder(BoundScope parent, FunctionSymbol function)
        {
            _scope = new BoundScope(parent);
            _function = function;

            if (function != null)
            {
                foreach (var p in function.Parameters)
                    _scope.TryDeclareVariable(p);
            }
        }

        public static BoundGlobalScope BindGlobalScope(BoundGlobalScope previous, ImmutableArray<SyntaxTree> syntaxTrees)
        {
            var parentScope = CreateParentScope(previous);
            var binder = new Binder(parentScope,null);

            var functionDecs = syntaxTrees.SelectMany(st => st.Root.Members).OfType<FunctionDeclarationSyntax>();

            foreach (var function in functionDecs)
                binder.BindFunctionDeclaration(function);

            var globalStatements = syntaxTrees.SelectMany(st => st.Root.Members).OfType<GlobalStatementSyntax>();

            var statements = ImmutableArray.CreateBuilder<BoundStatement>();
            foreach (var globalStatement in globalStatements)
            {
                var statement = binder.BindStatement(globalStatement.Statement);
                statements.Add(statement);
            }

            var functions = binder._scope.GetDeclaredFunctions();
            var variables = binder._scope.GetDeclaredVariables();
            var diagnostics = binder.Diagnostics.ToImmutableArray();

            if (previous != null)
                diagnostics = diagnostics.InsertRange(0, previous.Diagnostics);

            return new BoundGlobalScope(previous, diagnostics, functions, variables, statements.ToImmutable());
        }

        public static BoundProgram BindProgram(BoundGlobalScope globalScope)
        {
            var parentScope = CreateParentScope(globalScope);

            var functionBodies = ImmutableDictionary.CreateBuilder<FunctionSymbol, BoundBlockStatement>();
            var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();

            var scope = globalScope;

            while (scope != null)
            {
                foreach (var function in scope.Functions)
                {
                    var binder = new Binder(parentScope, function);
                    var body = binder.BindStatement(function.Declaration.Body);
                    var loweredBody = Lowerer.Lower(body);

                    if (function.ReturnType != TypeSymbol.Void && !ControlFlowGraph.AllPathsReturn(loweredBody))
                        binder._diagnostics.ReportAllPathsMustReturn(function.Declaration.Identifier.Location);

                    functionBodies.Add(function, loweredBody);

                    diagnostics.AddRange(binder.Diagnostics);
                }

                scope = scope.Previous;
            }
            var statement = Lowerer.Lower(new BoundBlockStatement(globalScope.Statements));

            return new BoundProgram(diagnostics.ToImmutable(), functionBodies.ToImmutable(), statement);
        }

        private void BindFunctionDeclaration(FunctionDeclarationSyntax syntax)
        {
            var parameters = ImmutableArray.CreateBuilder<ParameterSymbol>();

            var seenParameterNames = new HashSet<string>();

            foreach (var parameterSyntax in syntax.Parameters)
            {
                var parameterName = parameterSyntax.Identifier.Text;
                var parameterType = BindTypeClause(parameterSyntax.Type);
                if (!seenParameterNames.Add(parameterName))
                {
                    _diagnostics.ReportParameterAlreadyDeclared(parameterSyntax.Location, parameterName);
                }
                else
                {
                    var parameter = new ParameterSymbol(parameterName, parameterType);
                    parameters.Add(parameter);
                }
            }

            var type = BindTypeClause(syntax.Type) ?? TypeSymbol.Void;

            var function = new FunctionSymbol(syntax.Identifier.Text, parameters.ToImmutable(), type, syntax);
            if (function.Declaration.Identifier.Text != null &&!_scope.TryDeclareFunction(function))
                _diagnostics.ReportSymbolAlreadyDeclared(syntax.Identifier.Location, function.Name);
        }

        private static BoundScope CreateParentScope(BoundGlobalScope previous)
        {
            var stack = new Stack<BoundGlobalScope>();
            while(previous != null)
            {
                stack.Push(previous);
                previous = previous.Previous;
            }

            var parent = CreateRootScope();

            while (stack.Count > 0)
            {
                previous = stack.Pop();
                var scope = new BoundScope(parent);
                foreach (var f in previous.Functions)
                    scope.TryDeclareFunction(f);
                foreach (var v in previous.Variables)
                    scope.TryDeclareVariable(v);
                parent = scope;
            }
            return parent;
        }

        private static BoundScope CreateRootScope()
        {
            var result = new BoundScope(null);

            foreach(var func in BuiltInFunctions.GetAll())
                result.TryDeclareFunction(func);

            return result;
        }

        public DiagnosticBag Diagnostics => _diagnostics;

        private BoundStatement BindStatement(StatementSyntax syntax)
        {
            switch (syntax.Kind)
            {
                case SyntaxKind.BlockStatement:
                    return BindBlockStatement((BlockStatementSyntax)syntax);
                case SyntaxKind.VariableDeclaration:
                    return BindVariableDeclaration((VariableDeclarationSyntax)syntax);
                case SyntaxKind.IfStatement:
                    return BindIfStatement((IfStatementSyntax)syntax);
                case SyntaxKind.WhileStatement:
                    return BindWhileStatement((WhileStatementSyntax)syntax);
                case SyntaxKind.DoWhileStatement:
                    return BindDoWhileStatement((DoWhileStatementSyntax)syntax);
                case SyntaxKind.ForStatement:
                    return BindForStatement((ForStatementSyntax)syntax);
                case SyntaxKind.ExpressionStatement:
                    return BindExpressionStatement((ExpressionStatementSyntax)syntax);
                case SyntaxKind.ContinueStatement:
                    return BindContinueStatement((ContinueStatementSyntax)syntax);
                case SyntaxKind.BreakStatement:
                    return BindBreakStatement((BreakStatementSyntax)syntax);
                case SyntaxKind.ReturnStatement:
                    return BindReturnStatement((ReturnStatementSyntax)syntax);
                default:
                    throw new Exception($"Unexpected syntax {syntax.Kind}");
            }
        }

        private BoundStatement BindReturnStatement(ReturnStatementSyntax syntax)
        {
            var expression = syntax.Expression == null ? null : BindExpression(syntax.Expression);
            if (_function == null)
            {
                _diagnostics.ReportInvalidReturn(syntax.ReturnKeyword.Location);
            }
            else
            {
                if (_function.ReturnType == TypeSymbol.Void)
                {
                    if(expression!=null)
                        _diagnostics.ReportInvalidReturnExpression(syntax.Expression.Location, _function.Name);
                }
                else
                {
                    if(expression==null)
                        _diagnostics.ReportMissingReturnExpression(syntax.ReturnKeyword.Location, _function.ReturnType);
                    else
                    {
                        expression = BindConversion(syntax.Expression.Location, expression, _function.ReturnType);
                    }
                }
            }

            return new BoundReturnStatement(expression);
        }

        private BoundStatement BindBlockStatement(BlockStatementSyntax syntax)
        {
            var statements = ImmutableArray.CreateBuilder<BoundStatement>();
            _scope = new BoundScope(_scope);

            foreach(var statementSyntax in syntax.Statements)
            {
                var statement = BindStatement(statementSyntax);
                statements.Add(statement);
            }

            _scope = _scope.Parent;

            return new BoundBlockStatement(statements.ToImmutable());
        }

        private BoundStatement BindVariableDeclaration(VariableDeclarationSyntax syntax)
        {
            var isReadOnly = syntax.KeywordToken.Kind == SyntaxKind.ConstKeyword;
            var type = BindTypeClause(syntax.TypeClause);
            var initializer = BindExpression(syntax.Initializer);
            var variableType = type ?? initializer.Type;
            var variable = BindVariable(syntax.Identifier, isReadOnly, variableType);
            var convertedInitializer = BindConversion(syntax.Initializer.Location, initializer, variableType);

            return new BoundVariableDeclaration(variable, convertedInitializer);
        }

        private TypeSymbol BindTypeClause(TypeClauseSyntax syntax)
        {
            if (syntax == null)
                return null;

            var type = LookUpType(syntax.Identifier.Text);
            if (type == null)
                _diagnostics.ReportUndefinedType(syntax.Identifier.Location, syntax.Identifier.Text);

            return type;
        }

        private BoundStatement BindIfStatement(IfStatementSyntax syntax)
        {
            var condition = BindExpression(syntax.Condition, TypeSymbol.Bool);
            var thenStatement = BindStatement(syntax.ThenStatement);
            var elseStatement = syntax.ElseClause == null ? null : BindStatement(syntax.ElseClause.ElseStatement);
            return new BoundIfStatement(condition, thenStatement, elseStatement);
        }
        private BoundStatement BindWhileStatement(WhileStatementSyntax syntax)
        {
            var condition = BindExpression(syntax.Condition, TypeSymbol.Bool);
            var body = BindLoopBody(syntax.Body, out var breakLabel, out var continueLabel);
            return new BoundWhileStatement(condition, body, breakLabel, continueLabel);
        }


        private BoundStatement BindDoWhileStatement(DoWhileStatementSyntax syntax)
        {
            var body = BindLoopBody(syntax.Body, out var breakLabel, out var continueLabel);
            var condition = BindExpression(syntax.Condition, TypeSymbol.Bool);
            return new BoundDoWhileStatement(body, condition, breakLabel, continueLabel);
        }

        private BoundStatement BindForStatement(ForStatementSyntax syntax)
        {
            var lowerBound = BindExpression(syntax.LowerBound, TypeSymbol.Int);
            var upperBound = BindExpression(syntax.UpperBound, TypeSymbol.Int);

            _scope = new BoundScope(_scope);

            var variable = BindVariable(syntax.Identifier,isReadonly:true, TypeSymbol.Int);
            var body = BindLoopBody(syntax.Body, out var breakLabel, out var continueLabel);

            _scope = _scope.Parent;

            return new BoundForStatement(variable, lowerBound, upperBound, body, breakLabel, continueLabel);
        }

        private BoundStatement BindLoopBody(StatementSyntax body, out BoundLabel breakLabel, out BoundLabel continueLabel)
        {
            _labelCounter++;
            breakLabel = new BoundLabel($"break{_labelCounter}");
            continueLabel = new BoundLabel($"continue{_labelCounter}");

            _loopStack.Push((breakLabel, continueLabel));
            var boundBody = BindStatement(body);
            _loopStack.Pop();

            return boundBody;
        }


        private BoundStatement BindBreakStatement(BreakStatementSyntax syntax)
        {
            if (_loopStack.Count == 0)
            {
                _diagnostics.ReportInvalidBreakOrContinue(syntax.Keyword.Location, syntax.Keyword.Text);
                return BindErrorStatement();
            }

            var breakLabel = _loopStack.Peek().BreakLabel;
            return new BoundGotoStatement(breakLabel);
        }


        private BoundStatement BindContinueStatement(ContinueStatementSyntax syntax)
        {
            if (_loopStack.Count == 0)
            {
                _diagnostics.ReportInvalidBreakOrContinue(syntax.Keyword.Location, syntax.Keyword.Text);
                return BindErrorStatement();
            }

            var continueLabel = _loopStack.Peek().ContinueLabel;
            return new BoundGotoStatement(continueLabel);
        }

        private BoundStatement BindErrorStatement()
        {
            return new BoundExpressionStatement(new BoundErrorExpression());
        }

        private BoundStatement BindExpressionStatement(ExpressionStatementSyntax syntax)
        {
            var expression = BindExpression(syntax.Expression, canBeVoid: true);
            return new BoundExpressionStatement(expression);
        }

        private BoundExpression BindExpression(ExpressionSyntax syntax, TypeSymbol targetType)
        {
            return BindConversion(syntax, targetType);
        }

        private BoundExpression BindExpression(ExpressionSyntax syntax, bool canBeVoid = false)
        {
            var result = BindExpressionInternal(syntax);
            if (!canBeVoid && result.Type == TypeSymbol.Void)
            {
                _diagnostics.ReportExpressionMustHaveValue(syntax.Location);
                return new BoundErrorExpression();
            }

            return result;
        }

        private BoundExpression BindExpressionInternal(ExpressionSyntax syntax)
        {
            switch(syntax.Kind)
            {
                case SyntaxKind.ParenthesizedExpression:
                    return BindParenthesizedExpression((ParenthesizedExpressionSyntax)syntax);
                case SyntaxKind.LiteralExpression:
                    return BindLiteralExpression((LiteralExpressionSyntax)syntax);
                case SyntaxKind.NameExpression:
                    return BindNameExpression((NameExpressionSyntax)syntax);
                case SyntaxKind.AssignmentExpression:
                    return BindAssignmentExpression((AssignmentExpressionSyntax)syntax);
                case SyntaxKind.UnaryExpression:
                    return BindUnaryExpression((UnaryExpressionSyntax)syntax);
                case SyntaxKind.BinaryExpression:
                    return BindBinaryExpression((BinaryExpressionSyntax)syntax);
                case SyntaxKind.CallExpression:
                    return BindCallExpression((CallExpressionSyntax)syntax);
                case SyntaxKind.StringIndexExpression:
                    return BindStringIndexExpression((StringIndexExpressionSyntax)syntax);
                case SyntaxKind.NameIndexExpressoin:
                    return BindNameIndexExpressoin((NameIndexExpressionSyntax)syntax);
                case SyntaxKind.ParenthesizedIndexExpression:
                    return BindParenthesizedIndexExpression((ParenthesizedIndexExpressionSyntax)syntax);
                default:
                    throw new Exception($"Unexpected syntax {syntax.Kind}");
            }
        }

        private BoundExpression BindParenthesizedIndexExpression(ParenthesizedIndexExpressionSyntax syntax)
        {
            var boundExpression = BindParenthesizedExpression(syntax.ParenthesizedExpressionSyntax);
            if (boundExpression.Type != TypeSymbol.String)
            {
                _diagnostics.ReportIndexingNotAllowedForType(syntax.ParenthesizedExpressionSyntax.Location, boundExpression.Type);
            }
            var indexExpression = BindExpression(syntax.Expression);
            return new BoundIndexExpression(boundExpression, indexExpression, syntax.ParenthesizedExpressionSyntax.Location);
        }

        private BoundExpression BindNameIndexExpressoin(NameIndexExpressionSyntax syntax)
        {
            var boundName = BindNameExpression(syntax.NameExpression);
            if (boundName.Type != TypeSymbol.String)
            {
                _diagnostics.ReportIndexingNotAllowedForType(syntax.NameExpression.Location, boundName.Type);
            }
            var indexExpression = BindExpression(syntax.Expression);
            return new BoundIndexExpression(boundName, indexExpression, syntax.NameExpression.Location);
        }

        private BoundExpression BindStringIndexExpression(StringIndexExpressionSyntax syntax)
        {
            var boundString = BindExpression(syntax.StringExpression);
            var indexExpression = BindExpression(syntax.Expression);
            return new BoundIndexExpression(boundString, indexExpression, syntax.StringExpression.Location);
        }

        private BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax)
        {
            return BindExpression(syntax.Expression);
        }

        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            var value = syntax.Value ?? 0;
            return new BoundLiteralExpression(value);
        }
        private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;
            if (syntax.IdentifierToken.IsMissing)
            {
                return new BoundErrorExpression();
            }

            if (!_scope.TryLookUpVariable(name, out var variable))
            {
                _diagnostics.ReportUndefinedName(syntax.IdentifierToken.Location, name);
                return new BoundErrorExpression();
            }
            return new BoundVariableExpression(variable);
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;
            var boundExpression = BindExpression(syntax.Expression);

            if(!_scope.TryLookUpVariable(name,out var variable))
            {
                _diagnostics.ReportUndefinedName(syntax.IdentifierToken.Location, name);
                return boundExpression;
            }

            if (variable.IsReadonly)
                _diagnostics.ReportCannotAssign(syntax.EqualsToken.Location, name);
            var convertedExpression = BindConversion(syntax.Expression.Location, boundExpression, variable.Type);

            return new BoundAssignmentExpression(variable, convertedExpression);
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var boundOperand = BindExpression(syntax.Operand);
            if (boundOperand.Type == TypeSymbol.Error)
                return new BoundErrorExpression();

            var boundOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);
            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Location, syntax.OperatorToken.Text, boundOperand.Type);
                return new BoundErrorExpression();
            }

            return new BoundUnaryExpression(boundOperator, boundOperand);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.Left);
            var boundRight = BindExpression(syntax.Right);

            if (boundLeft.Type == TypeSymbol.Error || boundRight.Type == TypeSymbol.Error)
                return new BoundErrorExpression();

            var boundOperator = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);
            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Location, syntax.OperatorToken.Text, boundLeft.Type, boundRight.Type);
                return new BoundErrorExpression();
            }

            return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
        }

        private BoundExpression BindCallExpression(CallExpressionSyntax syntax)
        {
            if (syntax.Arguments.Count == 1 && LookUpType(syntax.Identifier.Text) is TypeSymbol type)
                return BindConversion(syntax.Arguments[0], type, allowExplicit:true);

            var boundArguments = ImmutableArray.CreateBuilder<BoundExpression>();

            foreach (var argument in syntax.Arguments)
            {
                var boundArgument = BindExpression(argument);
                boundArguments.Add(boundArgument);
            }

            if (!_scope.TryLookUpFunction(syntax.Identifier.Text, out var function))
            {
                _diagnostics.ReportUndefinedFunction(syntax.Identifier.Location, syntax.Identifier.Text);
                return new BoundErrorExpression();
            }

            if (syntax.Arguments.Count != function.Parameters.Length)
            {
                TextSpan span;
                if (syntax.Arguments.Count > function.Parameters.Length)
                {
                    SyntaxNode firstExceedingNode;
                    if (function.Parameters.Length > 0)
                        firstExceedingNode = syntax.Arguments.GetSeperator(function.Parameters.Length - 1);
                    else
                        firstExceedingNode = syntax.Arguments[0];
                    var lastExceedingArgument = syntax.Arguments[syntax.Arguments.Count - 1];
                    span = TextSpan.FromBounds(firstExceedingNode.Span.Start, lastExceedingArgument.Span.End);
                }
                else
                {
                    span = syntax.ClosedParenthesisToken.Span;
                }
                var location = new TextLocation(syntax.SyntaxTree.Text, span);
                _diagnostics.ReportWrongArgumentCount(location, function.Name, function.Parameters.Length, syntax.Arguments.Count);
                return new BoundErrorExpression();
            }

            bool hasErrors = false;

            for (var i = 0; i < syntax.Arguments.Count; i++)
            {
                var argument = boundArguments[i];
                var parameter = function.Parameters[i];

                if (argument.Type != parameter.Type)
                {
                    if (argument.Type != TypeSymbol.Error)
                        _diagnostics.ReportWrongArgumentType(syntax.Arguments[i].Location, parameter.Name, parameter.Type, argument.Type);
                    hasErrors = true;
                }

            }
            if (hasErrors)
                return new BoundErrorExpression();

            return new BoundCallExpression(function, boundArguments.ToImmutable(), syntax.Identifier.Location);
        }

        private BoundExpression BindConversion(ExpressionSyntax syntax, TypeSymbol type, bool allowExplicit = false)
        {
            var expression = BindExpression(syntax);
            return BindConversion(syntax.Location, expression, type,allowExplicit);
        }

        private BoundExpression BindConversion(TextLocation diagnosticLocation, BoundExpression expression, TypeSymbol type, bool allowExplicit=false)
        {
            var conversion = Conversion.Classify(expression.Type, type);

            if (!conversion.Exists)
            {
                if (expression.Type != TypeSymbol.Error && type != TypeSymbol.Error)
                    _diagnostics.ReportCannotConvert(diagnosticLocation, expression.Type, type);
                return new BoundErrorExpression();
            }

            if(!allowExplicit &&conversion.IsExplicit)
                _diagnostics.ReportCannotConvertImplicitly(diagnosticLocation, expression.Type, type);

            if (conversion.IsIdentity)
                return expression;
            return new BoundConversionExpression(type, expression, diagnosticLocation);
        }

        private VariableSymbol BindVariable(SyntaxToken identifier,bool isReadonly, TypeSymbol type)
        {
            var name = identifier.Text??"?";
            var declare = !identifier.IsMissing;
            var variable = _function == null
                    ? (VariableSymbol)new GlobalVariableSymbol(name, isReadonly, type)
                    : new LocalVariableSymbol(name, isReadonly, type);
            if (declare && !_scope.TryDeclareVariable(variable))
                _diagnostics.ReportSymbolAlreadyDeclared(identifier.Location, name);
            return variable;
        }

        private TypeSymbol LookUpType(string name)
        {
            switch (name)
            {
                case "bool":
                    return TypeSymbol.Bool;
                case "int":
                    return TypeSymbol.Int;
                case "string":
                    return TypeSymbol.String;
                case "decimal":
                    return TypeSymbol.Decimal;
                default:
                    return null;
            }
        }
    }
}
