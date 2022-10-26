#nullable enable

using Parser;

namespace Tests.TokenizerTests;

[TestClass]
public class WhiteSpaceTokenTests : TokenizerTests
{
    [DataTestMethod()]
    [DataRow("      ", false)]
    [DataRow("  \n    ", true)]
    public void TestSuccess(string text, bool hasnewline)
    {
        var ws = PerformSuccessfulTest<WhitespaceToken>(text, WhitespaceToken.TryConsume);
        Assert.AreEqual(hasnewline, ws.HasNewline);
    }

    [DataTestMethod()]
    [DataRow("")]
    public void TestFailed(string text)
    {
        TokenizerTests.PerformFailureTest<WhitespaceToken>(text, WhitespaceToken.TryConsume);
    }
}
