using Pital.CodeAnalysis.Syntax;
using System.Collections.Immutable;
using System.Linq;

namespace Pital.CodeAnalysis.Symbols
{
    public sealed class FunctionSymbol : Symbol
    {
        public FunctionSymbol(string name, ImmutableArray<ParameterSymbol> parameter, TypeSymbol returnType,FunctionDeclarationSyntax declaration=null)
            :base(name)
        {
            Parameters = parameter;
            ReturnType = returnType;
            Declaration = declaration;
        }

        public override SymbolKind Kind => SymbolKind.Fucntion;

        public ImmutableArray<ParameterSymbol> Parameters { get; }
        public TypeSymbol ReturnType { get; }
        public FunctionDeclarationSyntax Declaration { get; }
    }
}
