using System;
using Pital.CodeAnalysis.Binding;
using Pital.CodeAnalysis.Syntax;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.IO;

namespace Pital.CodeAnalysis
{
    public sealed class Compilation
    {
        private BoundGlobalScope _globalScope;
        public Compilation(SyntaxTree syntaxTree)
            :this(null,syntaxTree)
        {
        }

        private Compilation(Compilation previous,SyntaxTree syntaxTree)
        {
            Previous = previous;
            SyntaxTree = syntaxTree;
        }

        public Compilation Previous { get; }
        public SyntaxTree SyntaxTree { get; }

        internal BoundGlobalScope GlobalScope
        {
            get
            {
                if (_globalScope == null)
                {
                    var globalScope = Binder.BindGlobalScope(Previous?.GlobalScope,SyntaxTree.Root);
                    // if we assign Binder.BindGlobalScope(Syntax.Root) directly to _globalScope the code make becomes thread-unsafe
                    // making sure the global scope is thread-safe
                    Interlocked.CompareExchange(ref _globalScope, globalScope, null);
                }
                return _globalScope;
            }
        }

        public Compilation ContinueWith(SyntaxTree syntaxTree)
        {
            return new Compilation(this, syntaxTree);
        }

        public EvaluationResult Evaluate(Dictionary<VariableSymbol,object> variables)
        {

            var diagnostics = SyntaxTree.Diagnostics.Concat(GlobalScope.Diagnostics).ToImmutableArray();
            if (diagnostics.Any())
            {
                return new EvaluationResult(diagnostics.ToImmutableArray(), null);
            }

            var evaluator = new Evaluator(GlobalScope.Statement, variables);
            var value = evaluator.Evaluate();

            return new EvaluationResult(ImmutableArray<Diagnostic>.Empty, value);
        }

        public void EmitTree(TextWriter writer)
        {
            GlobalScope.Statement.WriteTo(writer);
        }
    }
}
