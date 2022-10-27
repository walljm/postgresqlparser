#nullable enable

using Parser;

namespace Tests.TokenizerTests;

[TestClass]
public class OperatorTokenTests : TokenizerTests
{
    [DataTestMethod()]
    [DataRow("+", "+")]
    [DataRow("-", "-")]
    [DataRow("/", "/")]
    [DataRow("*", "*")]
    [DataRow(">", ">")]
    [DataRow("<", "<")]
    [DataRow("=", "=")]
    [DataRow("~", "~")]
    [DataRow("!", "!")]
    [DataRow("@", "@")]
    [DataRow("%", "%")]
    [DataRow("#", "#")]
    [DataRow("^", "^")]
    [DataRow("|", "|")]
    [DataRow("?", "?")]
    [DataRow(">=", ">=")]
    [DataRow("<=", "<=")]
    [DataRow("=>", "=>")]
    [DataRow("=<", "=<")]
    [DataRow("$", "$")]
    [DataRow("(", "(")]
    [DataRow(")", ")")]
    [DataRow("[", "[")]
    [DataRow("]", "]")]
    [DataRow(",", ",")]
    [DataRow(";", ";")]
    [DataRow(":", ":")]
    [DataRow("*", "*")]
    [DataRow(".", ".")]
    public void OperatorTokenTest(string text, string expected)
    {
        PerformSuccessfulTest<OperatorToken>(text, expected, OperatorToken.TryConsume);
    }
}
