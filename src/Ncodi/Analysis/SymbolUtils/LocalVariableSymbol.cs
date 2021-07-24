namespace Ncodi.CodeAnalysis.Symbols
{
    public class LocalVariableSymbol : VariableSymbol
    {
        internal LocalVariableSymbol(string name, bool isReadonly, TypeSymbol type)
            : base(name, isReadonly, type)
        {

        }
        public override SymbolKind Kind => SymbolKind.LocalVariable;
    }
}
