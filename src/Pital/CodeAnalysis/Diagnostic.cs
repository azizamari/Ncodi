using Ncodi.CodeAnalysis.Text;

namespace Ncodi.CodeAnalysis
{
    public sealed class Diagnostic
    {
        public Diagnostic(TextSpan span,string message)
        {
            Span = span;
            Message = message;
        }

        public TextSpan Span { get; }
        public string Message { get; }

        public override string ToString() => Message;
    }
}
