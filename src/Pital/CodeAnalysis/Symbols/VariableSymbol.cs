using System;

namespace Pital.CodeAnalysis.Symbols
{   
    public sealed class VariableSymbol : Symbol
    {
        internal VariableSymbol(string name, bool isReadonly, TypeSymbol type)
            : base(name)
        {
            IsReadonly = isReadonly;
            Type = type;
        }

        public bool IsReadonly { get; }
        public TypeSymbol Type { get; }

        public override SymbolKind Kind => SymbolKind.Variable;

        public override string ToString() => Name;
    }
}
