using Parser;

namespace Tests
{
    [TestClass]
    public class TokenizerTests
    {
        [DataTestMethod()]
        [DataRow("table", "table", -1)]
        [DataRow("table1", "table1", -1)]
        [DataRow("table_name", "table_name", -1)]
        [DataRow("_table", "_table", -1)]
        [DataRow("1table", null, (int)'1')]
        public void IdentifierTokenTest1(string text, string expected, int current)
        {
            using var reader = new StringReader(text);
            IdentifierToken.TryConsume(reader, out var actual);
            Assert.AreEqual(expected, actual?.Value);
            Assert.AreEqual(current, reader.Read());
        }

        [DataTestMethod()]
        [DataRow("'foo'", "'foo'", -1)]
        [DataRow("'foo' \n 'bar'", "'foobar'", -1)]
        [DataRow("'fo\\'o' \n 'ba\\'r'", "'fo\\'oba\\'r'", -1)]
        [DataRow("'fo\\'o'", "'fo\\'o'", -1)]
        public void StringLiteralTokenTest(string text, string expected, int current)
        {
            using var reader = new StringReader(text);
            StringLiteralToken.TryConsume(reader, out var actual);
            Assert.AreEqual(expected, actual?.Value);
            Assert.AreEqual(current, reader.Read());
        }

        [DataTestMethod()]
        [DataRow("      ", true, false)]
        [DataRow("  \n    ", true, true)]
        [DataRow("", false, false)]
        public void WhitespaceTokenTest1(string text, bool haswhitespace, bool hasnewline)
        {
            using var reader = new StringReader(text);
            var ws = WhitespaceToken.Consume(reader);
            Assert.AreEqual(hasnewline, ws.HasNewline);
            Assert.AreEqual(haswhitespace, ws.HasWhitespace);
            Assert.AreEqual(-1, reader.Read());
        }

        [DataTestMethod()]
        [DataRow("\"foo\"", "\"foo\"", -1)]
        [DataRow("\"foasfd!@#$%^(*&_*(&*&^_+'o\"", "\"foasfd!@#$%^(*&_*(&*&^_+'o\"", -1)]
        public void QuotedItentifierTokenTest(string text, string expected, int current)
        {
            using var reader = new StringReader(text);
            QuotedIdentifier.TryConsume(reader, out var actual);
            Assert.AreEqual(expected, actual?.Value);
            Assert.AreEqual(current, reader.Read());
        }

        [DataTestMethod()]
        [DataRow("+", "+", true)]
        [DataRow("-", "-", true)]
        [DataRow("/", "/", true)]
        [DataRow("*", "*", true)]
        [DataRow(">", ">", true)]
        [DataRow("<", "<", true)]
        [DataRow("=", "=", true)]
        [DataRow("~", "~", true)]
        [DataRow("!", "!", true)]
        [DataRow("@", "@", true)]
        [DataRow("%", "%", true)]
        [DataRow("#", "#", true)]
        [DataRow("^", "^", true)]
        [DataRow("|", "|", true)]
        [DataRow("?", "?", true)]
        [DataRow(">=", ">=", true)]
        [DataRow("<=", "<=", true)]
        [DataRow("=>", "=>", true)]
        [DataRow("=<", "=<", true)]
        [DataRow("$", "$", true)]
        [DataRow("(", "(", true)]
        [DataRow(")", ")", true)]
        [DataRow("[", "[", true)]
        [DataRow("]", "]", true)]
        [DataRow(",", ",", true)]
        [DataRow(";", ";", true)]
        [DataRow(":", ":", true)]
        [DataRow("*", "*", true)]
        [DataRow(".", ".", true)]
        public void OperatorTokenTest(string text, string expected, bool isOperator)
        {
            using var reader = new StringReader(text);
            var didConsume = OperatorToken.TryConsume(reader, out var actual);
            Assert.AreEqual(isOperator, didConsume);
            if (didConsume)
            {
                Assert.AreEqual(expected, actual?.Value);
            }
        }

        [DataTestMethod()]
        [DataRow("42", "42", true)]
        [DataRow("3.5", "3.5", true)]
        [DataRow("4.a", "4.", true)]
        [DataRow(".001", ".001", true)]
        [DataRow("5e2", "5e2", true)]
        [DataRow("1.925e-3 ", "1.925e-3", true)]
        public void NumericTokenTest(string text, string expected, bool didConsume)
        {
            using var reader = new StringReader(text);
            var didConsumeActual = NumericToken.TryConsume(reader, out var actual);
            Assert.AreEqual(didConsumeActual, didConsume);
            if (didConsumeActual)
            {
                Assert.AreEqual(expected, actual?.Value);
            }
        }
    }
}