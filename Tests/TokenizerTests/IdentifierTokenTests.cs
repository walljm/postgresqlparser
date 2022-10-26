#nullable enable

using Parser;

namespace Tests.TokenizerTests;

[TestClass]
public class IdentifierTokenTests : TokenizerTests
{
    [DataTestMethod()]
    [DataRow("jwall", "jwall")]
    [DataRow("table1", "table1")]
    [DataRow("jason_wall", "jason_wall")]
    [DataRow("_table", "_table")]
    public void IdentifierTokenTest1(string text, string expected)
    {
        PerformSuccessfulTest<IdentifierToken>(text, expected, IdentifierToken.TryConsume);
    }

    [DataTestMethod()]
    [DataRow("1table")]
    public void IdentifierTokenTestFailed(string text)
    {
        PerformFailureTest<IdentifierToken>(text, IdentifierToken.TryConsume);
    }

    [DataTestMethod()]
    [DataRow("\"foo\"", "\"foo\"")]
    [DataRow("\"foasfd!@#$%^(*&_*(&*&^_+'o\"", "\"foasfd!@#$%^(*&_*(&*&^_+'o\"")]
    public void QuotedIdentifierTokenTest(string text, string expected)
    {
        PerformSuccessfulTest<QuotedIdentifier>(text, expected, IdentifierToken.TryConsumeQuoted);
    }
}
