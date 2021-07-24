﻿using System;
using Ncodi.CodeAnalysis.Binding;
using Ncodi.CodeAnalysis.Syntax;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.IO;
using Ncodi.CodeAnalysis.Lowering;
using Ncodi.CodeAnalysis.Symbols;
using System.Threading.Tasks;

namespace Ncodi.CodeAnalysis
{
    public sealed class Compilation
    {
        private BoundGlobalScope _globalScope;
        public Compilation(params SyntaxTree[] syntaxTrees)
            :this(null,syntaxTrees)
        {
        }

        private Compilation(Compilation previous, params SyntaxTree[] syntaxTrees)
        {
            Previous = previous;
            SyntaxTrees = syntaxTrees.ToImmutableArray();
        }

        public Compilation Previous { get; }
        public ImmutableArray<SyntaxTree> SyntaxTrees { get; }

        internal BoundGlobalScope GlobalScope
        {
            get
            {
                if (_globalScope == null)
                {
                    var globalScope = Binder.BindGlobalScope(Previous?.GlobalScope,SyntaxTrees);
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
        
        public EvaluationResult Evaluate(Dictionary<VariableSymbol,object> variables, bool useConsole=true, Func<Task<string>> GetInput=null, Action<string> send = null)
        {
            var parseDiagnostics = SyntaxTrees.SelectMany(st => st.Diagnostics);
            var diagnostics = parseDiagnostics.Concat(GlobalScope.Diagnostics).ToImmutableArray();
            if (diagnostics.Any())
                return new EvaluationResult(diagnostics.ToImmutableArray(), null);

            var program = Binder.BindProgram(GlobalScope);


            //var appPath = Environment.GetCommandLineArgs()[0];
            //var appDirectory = Path.GetDirectoryName(appPath);
            //var cfgPath = Path.Combine(appDirectory, "cfg.dot");
            //var cfgStatement = !program.Statement.Statements.Any() && program.Functions.Any()
            //                                 ? program.Functions.Last().Value
            //                                 : program.Statement;
            //var cfg = ControlFlowGraph.Create(cfgStatement);
            //using (var streamWriter = new StreamWriter(cfgPath))
            //    cfg.WriteTo(streamWriter);


            if (program.Diagnostics.Any())
                return new EvaluationResult(program.Diagnostics.ToImmutableArray(), null);

            var evaluator = new Evaluator(program, variables);
            var value = evaluator.Evaluate(useConsole,GetInput,send);
            if (evaluator.Diagnostics.Any())
                return new EvaluationResult(evaluator.Diagnostics.ToImmutableArray(), null);

            return new EvaluationResult(ImmutableArray<Diagnostic>.Empty, value, evaluator._outputLines);
        }

        public void EmitTree(TextWriter writer)
        {
            var program = Binder.BindProgram(GlobalScope);

            if (program.Statement.Statements.Any())
            {
                program.Statement.WriteTo(writer);
            }
            else
            {
                foreach (var functionBody in program.Functions)
                {
                    if (!GlobalScope.Functions.Contains(functionBody.Key))
                        continue;
                    
                    functionBody.Value.WriteTo(writer);
                }
            }
        }

    }
}
