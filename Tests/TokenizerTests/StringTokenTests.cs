#nullable enable

using Parser;

namespace Tests.TokenizerTests;

[TestClass]
public class StringTokenTests : TokenizerTests
{
    [DataTestMethod()]
    [DataRow("'foo'", "'foo'")]
    [DataRow("'foo' \n 'bar'", "'foobar'")]
    [DataRow("'fo\\'o' \n 'ba\\'r'", "'fo\\'oba\\'r'")]
    [DataRow("'fo\\'o'", "'fo\\'o'")]
    public void StringLiteralTokenTest(string text, string expected)
    {
        PerformSuccessfulTest<StringLiteralToken>(text, expected, StringLiteralToken.TryConsume);
    }
}
