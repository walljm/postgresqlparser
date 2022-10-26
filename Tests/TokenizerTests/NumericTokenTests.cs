#nullable enable

using Parser;

namespace Tests.TokenizerTests;

[TestClass]
public class NumericTokenTests : TokenizerTests
{
    [DataTestMethod()]
    [DataRow("42", "42")]
    [DataRow("3.5", "3.5")]
    [DataRow("4.a", "4.")]
    [DataRow(".001", ".001")]
    [DataRow("5e2", "5e2")]
    [DataRow("1.925e-3 ", "1.925e-3")]
    public void NumericTokenTest(string text, string expected)
    {
        PerformSuccessfulTest<NumericToken>(text, expected, NumericToken.TryConsume, verifyCompleted: false);
    }
}
