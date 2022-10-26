using System.Diagnostics.CodeAnalysis;
using Parser;

namespace Tests.TokenizerTests
{
    public abstract class TokenizerTests
    {
        protected delegate bool TryConsumeToken<T>(ReadOnlySpan<char> text, out int charsRead, [NotNullWhen(true)] out T? token);

        protected static T PerformSuccessfulTest<T>(
            ReadOnlySpan<char> text,
            TryConsumeToken<T> tryConsume,
            bool verifyCompleted = true
        )
            where T : Token
        {
            Assert.IsTrue(tryConsume(text, out var charsRead, out var token));
            if (verifyCompleted)
            {
                Assert.IsTrue(text[charsRead..].IsEmpty);
            }
            return token;
        }
        protected static T PerformSuccessfulTest<T>(
            ReadOnlySpan<char> text,
            string expected,
            TryConsumeToken<T> tryConsume,
            bool verifyCompleted = true
        )
            where T : Token
        {
            Assert.IsTrue(tryConsume(text, out var charsRead, out var actual));
            Assert.AreEqual(expected, actual.Value);
            if (verifyCompleted)
            {
                Assert.IsTrue(text[charsRead..].IsEmpty);
            }
            return actual;
        }

        protected static void PerformFailureTest<T>(
            string text,
            TryConsumeToken<T> tryConsume
        )
            where T : Token
        {
            Assert.IsFalse(tryConsume(text, out _, out _));
        }


    }
}
