using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Linq;

namespace Pital.CodeAnalysis.Symbols
{
    internal static class BuiltInFunctions
    {
        //public static readonly FunctionSymbol Print=new FunctionSymbol("ekteb", ImmutableArray.Create(new ParameterSymbol("text", TypeSymbol.String)),TypeSymbol.Void);
        //public static readonly FunctionSymbol Input=new FunctionSymbol("a9ra", ImmutableArray<ParameterSymbol>.Empty,TypeSymbol.String);

        public static readonly FunctionSymbol Print=new FunctionSymbol("print", ImmutableArray.Create(new ParameterSymbol("text", TypeSymbol.String)),TypeSymbol.Void);
        public static readonly FunctionSymbol Input=new FunctionSymbol("input", ImmutableArray<ParameterSymbol>.Empty,TypeSymbol.String);
        public static readonly FunctionSymbol Random = new FunctionSymbol("random", ImmutableArray.Create(new ParameterSymbol("max", TypeSymbol.Int)), TypeSymbol.Int);
        internal static IEnumerable<FunctionSymbol> GetAll() => typeof(BuiltInFunctions).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f=>f.FieldType==typeof(FunctionSymbol))
            .Select(f => (FunctionSymbol)f.GetValue(null));
    }
}
