using System;

namespace Pital.CodeAnalysis.Text
{
    public struct TextSpan
    {
        public TextSpan(int start, int length)
        {
            Start = start;
            Length = length;
        }

        public int Start { get; }
        public int Length { get; }
        public int End => Start + Length;

        public static TextSpan FromBounds(int start, int end)
        {
            return new TextSpan(start, end - start);
        }
        public override string ToString()
        {
            return $"{Start}..{End}";
        }
    }
}
