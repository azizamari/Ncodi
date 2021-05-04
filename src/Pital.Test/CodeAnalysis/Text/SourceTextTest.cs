using Xunit;
using Ncodi.CodeAnalysis.Text;

namespace Ncodi.Test.CodeAnalysis.Text
{
    public class SourceTextTest
    {
        [Theory]
        [InlineData(".",1)]
        [InlineData(".\r\n",2)]
        [InlineData(".\r\n\r\n", 3)]
        public void SourceText_IncludesLastLine(string text,int expectedLineCount)
        {
            var sourceText = SourceText.From(text);
            var actualLineCount = sourceText.Lines.Length;
            Assert.Equal(expectedLineCount, actualLineCount);
        }
    }
}

