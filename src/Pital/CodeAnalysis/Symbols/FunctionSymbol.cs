using System.Collections.Immutable;
using System.Linq;

namespace Pital.CodeAnalysis.Symbols
{
    public sealed class FunctionSymbol : Symbol
    {
        public FunctionSymbol(string name, ImmutableArray<ParameterSymbol> parameter, TypeSymbol returnType)
            :base(name)
        {
            Parameters = parameter;
            ReturnType = returnType;
        }

        public override SymbolKind Kind => SymbolKind.Fucntion;

        public ImmutableArray<ParameterSymbol> Parameters { get; }
        public TypeSymbol ReturnType { get; }
    }
}
