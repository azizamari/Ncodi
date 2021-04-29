namespace Pital.CodeAnalysis.Symbols
{
    public sealed class ParameterSymbol: VariableSymbol
    {
        public ParameterSymbol(string name, TypeSymbol type)
            :base(name,true,type)
        {
        }

        public override SymbolKind Kind => SymbolKind.Parameter;
    }
}
