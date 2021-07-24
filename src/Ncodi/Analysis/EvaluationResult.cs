using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Ncodi.CodeAnalysis
{
    public sealed class EvaluationResult
    {
        public EvaluationResult(ImmutableArray<Diagnostic> diagnostics,object value, List<string> outputLines=null)
        {
            Diagnostics = diagnostics;
            Value = value;
            OutputLines = outputLines;
        }

        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public object Value { get; }
        public List<string> OutputLines { get; }
    }
}
