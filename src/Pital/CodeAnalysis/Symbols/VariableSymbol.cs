using System;

namespace Pital.CodeAnalysis.Symbols
{   
    public sealed class VariableSymbol : Symbol
    {
        internal VariableSymbol(string name, bool isReadonly, Type type)
            : base(name)
        {
            IsReadonly = isReadonly;
            Type = type;
        }

        public bool IsReadonly { get; }
        public Type Type { get; }

        public override SymbolKind Kind => SymbolKind.VariableSymbol;

        public override string ToString() => Name;
    }
}
