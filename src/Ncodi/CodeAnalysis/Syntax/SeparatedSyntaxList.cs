using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Ncodi.CodeAnalysis.Syntax
{
    public abstract class SeparatedSyntaxList
    {
        public abstract ImmutableArray<SyntaxNode> GetWithSeparators();
    }

    public sealed class SeparatedSyntaxList<T> : SeparatedSyntaxList, IEnumerable<T>
        where T: SyntaxNode
    {
        private readonly ImmutableArray<SyntaxNode> _seperatorsAndNodes;

        public SeparatedSyntaxList(ImmutableArray<SyntaxNode> seperatorsAndNodes)
        {
            _seperatorsAndNodes = seperatorsAndNodes;
        }

        public int Count => (_seperatorsAndNodes.Length + 1) / 2;
        public T this[int index]=>(T) _seperatorsAndNodes[index*2];
        public SyntaxToken GetSeperator(int index)
        {
            if (index == Count - 1)
                return null;
            return (SyntaxToken)_seperatorsAndNodes[index * 2 + 1];
        }
        public override ImmutableArray<SyntaxNode> GetWithSeparators() => _seperatorsAndNodes;
        public IEnumerator<T> GetEnumerator()
        {
            for(var i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
