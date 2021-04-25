using Pital.CodeAnalysis.Binding;

namespace Pc
{
    internal static class Program
    {
        private static void Main()
        {
            var repl = new PitalRepl();
            repl.Run();
        }
    }
}
