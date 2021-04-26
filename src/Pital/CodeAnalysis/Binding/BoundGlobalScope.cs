using Pital.CodeAnalysis.Symbols;
using System.Collections.Immutable;

namespace Pital.CodeAnalysis.Binding
{
    internal sealed class BoundGlobalScope
    {
        public BoundGlobalScope(BoundGlobalScope previous, ImmutableArray<Diagnostic> diagnostics,ImmutableArray<VariableSymbol> variables, BoundStatement statement)
        {
            Previous = previous;
            Diagnostics = diagnostics;
            Variables = variables;
            Statement = statement;
        }

        public BoundGlobalScope Previous { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public ImmutableArray<VariableSymbol> Variables { get; }
        public BoundStatement Statement { get; }
    }
}
