#nullable enable

using Parser;

namespace Tests.TokenizerTests;

[TestClass]
public class NumericTokenTests : TokenizerTests
{
    [DataTestMethod]
    [DataRow("42", "42")]
    [DataRow("3.5", "3.5")]
    [DataRow(".001", ".001")]
    [DataRow("5e2", "5e2")]
    [DataRow("4.", "4.")]
    [DataRow("1.925e-3", "1.925e-3")]
    public void TestShouldConsumeEntireValue(string text, string expected)
    {
        PerformSuccessfulTest<NumericToken>(text, expected, NumericToken.TryConsume, verifyCompleted: true);
    }

    [DataTestMethod]
    [DataRow("4.a", "4.")]
    [DataRow("1.925e-3 ", "1.925e-3")]
    public void NumericTokenTest(string text, string expected)
    {
        PerformSuccessfulTest<NumericToken>(text, expected, NumericToken.TryConsume, verifyCompleted: false);
    }


    [DataTestMethod]
    [DataRow("1e")]
    [DataRow("1.2e")]
    [DataRow(".2e")]
    [DataRow("1.e")]
    public void At_least_one_digit_must_follow_exponent_marker(string text)
    {
        PerformFailureTest<NumericToken>(text, NumericToken.TryConsume);
    }

    [TestMethod]
    public void At_least_one_digit_must_be_before_or_after_decimal_point()
    {
        PerformFailureTest<NumericToken>(".", NumericToken.TryConsume);
    }

    [DataTestMethod]
    [DataRow("1+2")]
    [DataRow("1.+2")]
    [DataRow("1e++2")]
    public void Signs_are_only_allowed_in_prescribed_locations(string text)
    {
        PerformFailureTest<NumericToken>(".", NumericToken.TryConsume);
    }
}
