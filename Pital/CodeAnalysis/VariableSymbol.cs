using System;

namespace Pital.CodeAnalysis
{
    public sealed class VariableSymbol
    {
        internal VariableSymbol(string name, bool isReadonly, Type type)
        {
            Name = name;
            IsReadonly = isReadonly;
            Type = type;
        }

        public string Name { get; }
        public bool IsReadonly { get; }
        public Type Type { get; }
    }
}
