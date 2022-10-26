using System.Diagnostics.CodeAnalysis;
using Parser;

namespace Tests.TokenizerTests
{
    public abstract class TokenizerTests
    {
        protected delegate bool TryConsumeToken<T>(TextReader textReader, [NotNullWhen(true)] out T? token);

        protected static T PerformSuccessfulTest<T>(
            string text,
            TryConsumeToken<T> tryConsume,
            bool verifyCompleted = true
        )
            where T : Token
        {
            using var reader = new StringReader(text);
            Assert.IsTrue(tryConsume(reader, out var token));
            if (verifyCompleted)
            {
                Assert.AreEqual(-1, reader.Peek());
            }
            return token;
        }
        protected static T PerformSuccessfulTest<T>(
            string text,
            string expected,
            TryConsumeToken<T> tryConsume,
            bool verifyCompleted = true
        )
            where T : Token
        {
            using var reader = new StringReader(text);
            Assert.IsTrue(tryConsume(reader, out var actual));
            Assert.AreEqual(expected, actual.Value);
            if (verifyCompleted)
            {
                Assert.AreEqual(-1, reader.Peek());
            }
            return actual;
        }

        protected static void PerformFailureTest<T>(
            string text,
            TryConsumeToken<T> tryConsume
        )
            where T : Token
        {
            using var reader = new StringReader(text);
            Assert.IsFalse(tryConsume(reader, out _));
        }


    }
}
