using Ncodi.CodeAnalysis.Symbols;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Ncodi.CodeAnalysis.Binding
{
    internal sealed class BoundScope
    {
        private Dictionary<string,VariableSymbol> _variables=new Dictionary<string, VariableSymbol>();
        private Dictionary<string,FunctionSymbol> _functions=new Dictionary<string, FunctionSymbol>();

        public BoundScope(BoundScope parent)
        {
            Parent = parent;
        }
        public BoundScope Parent { get; }

        public bool TryDeclareVariable(VariableSymbol variable)
        {
            if (_variables == null)
                _variables = new Dictionary<string, VariableSymbol>();

            if (_variables.ContainsKey(variable.Name) || TryLookUpFunction(variable.Name, out var function))
            {
                return false;
            }
            _variables.Add(variable.Name, variable);
            return true;
        }

        public bool TryLookUpVariable(string name, out VariableSymbol variable)
        {
            variable = null;
            if (_variables!=null && _variables.TryGetValue(name,out variable))
                return true;
            if (Parent == null)
            {
                return false;
            }
            return Parent.TryLookUpVariable(name, out variable);
        }
        public bool TryDeclareFunction(FunctionSymbol function)
        {
            if(_functions==null)
                _functions = new Dictionary<string, FunctionSymbol>();

            if (_functions.ContainsKey(function.Name) || TryLookUpVariable(function.Name, out var variable))
            {
                return false;
            }
            _functions.Add(function.Name, function);
            return true;
        }

        public bool TryLookUpFunction(string name, out FunctionSymbol function)
        {
            function = null;
            if (_functions!=null&& _functions.TryGetValue(name, out function))
                return true;
            if (Parent == null)
                return false;
            return Parent.TryLookUpFunction(name, out function);
        }
        public ImmutableArray<VariableSymbol> GetDeclaredVariables()
        {
            if (_variables == null)
                return ImmutableArray<VariableSymbol>.Empty;
            return _variables.Values.ToImmutableArray();
        }
        public ImmutableArray<FunctionSymbol> GetDeclaredFunctions()
        {
            if (_functions == null)
                return ImmutableArray<FunctionSymbol>.Empty;
            return _functions.Values.ToImmutableArray();
        }
    }
}
