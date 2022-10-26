using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Parser
{
    public static partial class Tokenizer
    {


        public static IEnumerable<Token> Scan(string? text)
        {
            if (string.IsNullOrEmpty(text))
            {
                yield break;
            }
            var startIndex = 0;
            while (startIndex < text.Length)
            {
                if (!Tokenizer.TryConsumeToken(text.AsSpan()[startIndex..], out var charsRead, out var token))
                {
                    throw new Exception($"Unknown character in expression: {text[0]}");
                }
                startIndex += charsRead;
                yield return token;
            }
        }



        private static bool TryConsumeToken(
            ReadOnlySpan<char> text,
            out int charsRead,
            [NotNullWhen(true)] out Token? token
        )
        {
            token = ConsumeWhiteSpace(text, out charsRead)
                   ?? ConsumeNumeric(text, out charsRead)
                   ?? ConsumeOperator(text, out charsRead)
                   ?? ConsumeIdentifier(text, out charsRead)
                   ?? ConsumeString(text, out charsRead);
            return token is not null;
        }

        private static Token? ConsumeWhiteSpace(ReadOnlySpan<char> text, out int charsRead)
            => WhitespaceToken.TryConsume(text, out charsRead, out var token) ? token : default;

        private static Token? ConsumeNumeric(ReadOnlySpan<char> text, out int charsRead)
            => NumericToken.TryConsume(text, out charsRead, out var token) ? token : default;
        private static Token? ConsumeOperator(ReadOnlySpan<char> text, out int charsRead)
            => OperatorToken.TryConsume(text, out charsRead, out var token) ? token : default;
        private static Token? ConsumeIdentifier(ReadOnlySpan<char> text, out int charsRead)
            => IdentifierToken.TryConsume(text, out charsRead, out var token) ? token : default;
        private static Token? ConsumeString(ReadOnlySpan<char> text, out int charsRead)
            => StringLiteralToken.TryConsume(text, out charsRead, out var token) ? token : default;
    }

    public abstract record Token(string Value);

    // language types

    public record BitStringToken(string Value) : Token(Value);
    //tested
    public record NumericToken(string Value) : Token(Value)
    {
        public static bool IsValidStartChar(char c)
        {
            return char.IsDigit(c)
                || c == '.'
                ;
        }
        public static bool IsValidStartChar(int i)
        {
            return IsValidStartChar((char)i);
        }
        public static bool IsValidChar(char c)
        {
            return c is >= '0' and <= '9' or '.' or 'e' or '-' or '+';
        }
        public static bool IsValidChar(int i)
        {
            return IsValidChar((char)i);
        }

        /*
        42
        3.5
        4.
        .001
        5e2
        1.925e-3
        */


        public static bool TryConsume(string? text, [NotNullWhen(true)] out NumericToken? value)
            => TryConsume(text, out var charsRead, out value) && charsRead == text?.Length;
        public static bool TryConsume(ReadOnlySpan<char> text, out int charsRead, [NotNullWhen(true)] out NumericToken? value)
        {
            /*
            digits
            digits.[digits][e[+-]digits]
            [digits].digits[e[+-]digits]
            digitse[+-]digits

            where digits is one or more decimal digits (0 through 9). At least one digit must be before or after the decimal point,
            if one is used. At least one digit must follow the exponent marker (e), if one is present. There cannot be any spaces
            or other characters embedded in the constant. Note that any leading plus or minus sign is not actually considered part
            of the constant; it is an operator applied to the constant.
            */

            charsRead = default;
            value = default;

            var originalText = text;
            var wholeDigitCount = ReadDigits(ref text);
            var hasDecimal = TryConsumeChar(ref text, '.');
            var fractionalDigitCount = ReadDigits(ref text);
            var hasExponent = false;
            var hasExponentSign = false;
            var exponentDigitCount = 0;

            if (wholeDigitCount is 0 && fractionalDigitCount is 0)
            {
                charsRead = default;
                value = default;
                return false;
            }
            hasExponent = TryConsumeChar(ref text, 'e');
            if (hasExponent)
            {
                hasExponentSign = HasSign(ref text);
                exponentDigitCount = ReadDigits(ref text);
                if (exponentDigitCount is 0)
                {
                    return false;
                }
            }

            charsRead = wholeDigitCount
                        + BoolToInt(hasDecimal)
                        + fractionalDigitCount
                        + BoolToInt(hasExponent)
                        + BoolToInt(hasExponentSign)
                        + exponentDigitCount;
            value = new (new string(originalText[..charsRead]));
            return true;

            static int BoolToInt(bool value) => value ? 1 : 0;
            static int GetDigitLength(ReadOnlySpan<char> text)
            {
                for (var i = 0; i < text.Length; ++i)
                {
                    if (text[i] is not (>= '0' and <= '9'))
                        return i;
                }
                return text.Length;
            }
            static bool HasSign(ref ReadOnlySpan<char> text)
                => TryConsumeChar(ref text, '+')
                   || TryConsumeChar(ref text, '-');

            static bool TryConsumeChar(ref ReadOnlySpan<char> text, char ch)
            {
                if (text.IsEmpty || text[0] != ch)
                {
                    return false;
                }
                text = text[1..];
                return true;
            }
            static int ReadDigits(ref ReadOnlySpan<char> text)
            {
                var digitLength = GetDigitLength(text);
                text = text[digitLength..];
                return digitLength;
            }
        }

    }
    //tested
    public record OperatorToken(string Value) : Token(Value)
    {
        private static readonly HashSet<char> nextOperators = new(new[]
        {
            // boolean
            '<', '>', '=',
            ':',']','[',
        });

        private static readonly HashSet<char> operators = new(new[]
        {
            // boolean
            '<', '>', '=',
            // arithmatic
            '+', '-', '/','*',
            // delimiters
            '~', '!', '@', '#', '%', '^', '&', '|', '?', '`',
        });
        private static readonly HashSet<char> specialChars = new(new[]
        {
            // special characters
            '$','(',')','[',']',',',';',':','.',
        });

        public static bool IsNextOperatorChar(char c)
        {
            return nextOperators.Contains(c);
        }
        public static bool IsNextOperatorChar(int c)
        {
            return IsNextOperatorChar((char)c);
        }

        public static bool IsOperatorChar(char c)
        {
            return operators.Contains(c) || specialChars.Contains(c);
        }

        public static bool IsOperatorChar(int c)
        {
            return IsOperatorChar((char)c);
        }

        /*

        -- and /* cannot appear anywhere in an operator name, since they will be taken as the start of a comment.

        A multiple-character operator name cannot end in + or -, unless the name also contains at least one of
        these characters:

        ~ ! @ # % ^ & | ` ?

        For example, @- is an allowed operator name, but *- is not.

        This restriction allows PostgreSQL to parse SQL-compliant queries without requiring spaces between tokens.

         */

        public static bool TryConsume(string? text, [NotNullWhen(true)] out OperatorToken? value)
            => TryConsume(text, out var charsRead, out value) && charsRead == text?.Length;
        public static bool TryConsume(ReadOnlySpan<char> text, out int charsRead, out OperatorToken? value)
        {
            (value, charsRead) = (default, default);
            for (charsRead = 0; charsRead < text.Length; ++charsRead)
            {
                if (!IsValid(charsRead is 0, text[charsRead]))
                {
                    break;
                }
            }

            value = charsRead is 0
                ? null
                : new OperatorToken(new string(text[..charsRead]));
            return value is not null;

            static bool IsValid(bool isFirst, char ch)
            {
                return isFirst
                    ? IsOperatorChar(ch)
                    : IsNextOperatorChar(ch);
            }
        }
    };
    //tested
    public record IdentifierToken(string Value) : Token(Value)
    {
        public static readonly char QuotedDelimiter = '"';
        public bool IsKeyword()
        {
            return Tokenizer.Keywords.Contains(Value.ToUpper());
        }

        public bool IsInlineKeyword()
        {
            return Tokenizer.InlineKeywords.Contains(Value.ToUpper());
        }

        public static bool IsIdentifierChar(char c)
        {
            return char.IsLetterOrDigit(c)
                || c == '_'
                // we don't want to support '$' because they're technically illegal in names in the SQL standard.
                // || c == '$';
                ;
        }

        public static bool IsIdentifierBeginningChar(char c)
        {
            return char.IsLetter(c) || c == '_';
        }

        public static bool TryConsume(string? text, [NotNullWhen(true)] out IdentifierToken? value)
            => TryConsume(text, out var charsRead, out value) && charsRead == text?.Length;
        public static bool TryConsume(ReadOnlySpan<char> text, out int charsRead, [NotNullWhen(true)] out IdentifierToken? value)
        {
            value = default;
            if (TryConsumeQuoted(text, out charsRead, out var quotedIdentifier))
            {
                value = quotedIdentifier;
                return true;
            }
            for (charsRead = 0; charsRead < text.Length; ++charsRead)
            {
                if (!IsValid(charsRead is 0, text[charsRead]))
                {
                    break;
                }
            }

            if (charsRead is 0)
            {
                return false;
            }
            var valueString = new string(text[..charsRead]);
            value = Tokenizer.Keywords.Contains(valueString) switch
            {
                true when IsAllUppercase(valueString) => new KeywordToken(valueString),
                true => new KeywordToken(valueString.ToUpper()),
                _ => new IdentifierToken(valueString),
            };
            return true;

            static bool IsAllUppercase(ReadOnlySpan<char> text)
            {
                foreach (var ch in text)
                {
                    if (!char.IsUpper(ch))
                    {
                        return false;
                    }
                }
                return true;
            }
            static bool IsValid(bool isFirst, char ch)
            {
                return isFirst
                    ? IsIdentifierBeginningChar(ch)
                    : IsIdentifierChar(ch);
            }
        }


        // TODO: someday, lets support the U&"" unicode variation.
        // TODO: should we allow escaped quotes in this?
        public static bool TryConsumeQuoted(TextReader reader, [NotNullWhen(true)] out QuotedIdentifier? value)
        {
            if (!reader.CanRead() || reader.PeekChar() != QuotedDelimiter)
            {
                value = null;
                return false;
            }

            value = new QuotedIdentifier(reader.ReadWrappedString(QuotedDelimiter));
            return true;
        }

        public static bool TryConsumeQuoted(ReadOnlySpan<char> text, out int charsRead, [NotNullWhen(true)] out QuotedIdentifier? value)
        {
            (value, charsRead) = (default, default);
            if (text.IsEmpty || text[0] != QuotedDelimiter)
            {
                return false;
            }
            if (text[1..].IndexOf(QuotedDelimiter) is not (>= 0 and var insideLength))
            {
                return false;
            }
            charsRead = insideLength + 2; // Include 2 for the quotes
            value = new (text[..charsRead].ToString());
            return true;
        }
    };
    //tested
    public record KeywordToken(string Value) : IdentifierToken(Value);
    //tested
    public record QuotedIdentifier(string Value) : IdentifierToken(Value);
    //tested
    public record StringLiteralToken(string Value) : Token(Value)
    {
        public const char Delimiter = '\'';

        public static bool TryConsume(string? text, [NotNullWhen(true)] out StringLiteralToken? value)
            => TryConsume(text, out var charsRead, out value) && charsRead == text?.Length;

        // TODO: handle '' escaping
        // TODO: you should support the E'' escaped string format at some point.
        public static bool TryConsume(ReadOnlySpan<char> text, out int charsRead, [NotNullWhen(true)] out StringLiteralToken? value)
        {
            (charsRead, value) = (default, default);
            if (!GetStringPart(ref text, ref charsRead, out var firstPart))
            {
                return false;
            }
            if (!GetSubsequentPartRef(ref text, ref charsRead, out var subsequentPart))
            {
                // If there's only one part, just return that.
                charsRead = firstPart.Length;
                value = new (new string(firstPart));
                return true;
            }
            // according to postgres spec, we ignore two ticks with a new line between them.
            //  i.e. 'foo' \n 'bar' becomes 'foobar'

            var sb = new StringBuilder();
            sb.Append(Delimiter);
            sb.Append(firstPart[1..^1]); // Make sure we strip the quotes
            sb.Append(subsequentPart[1..^1]);
            while (GetSubsequentPartRef(ref text, ref charsRead, out subsequentPart))
            {
                sb.Append(subsequentPart[1..^1]);
            }
            sb.Append(Delimiter);
            value = new StringLiteralToken(sb.ToString());
            return true;

            static bool GetSubsequentPartRef(ref ReadOnlySpan<char> text, ref int charsRead, out ReadOnlySpan<char> outerStringPart)
            {
                outerStringPart = default;
                var textCopy = text;
                var charsReadCopy = charsRead;
                if (!StartsWithNewLine(ref textCopy, ref charsReadCopy))
                    return false;
                if(!GetStringPart(ref textCopy, ref charsReadCopy, out outerStringPart))
                    return false;
                text = textCopy;
                charsRead = charsReadCopy;
                return true;
            }

            static bool StartsWithNewLine(ref ReadOnlySpan<char> text, ref int charsRead)
            {
                // If text begins with leading whitespace, and that leading whitespace contains '\n', then:
                //     - Increment charsRead with the length of the leading whitespace
                //     - Trim the leading whitespace from text
                //     - Return true
                // Otherwise, return false
                var trimmedText = text.TrimStart();
                var trimLength = text.Length - trimmedText.Length;
                var leading = text[..trimLength];
                if (leading.Contains('\n') is false)
                {
                    return false;
                }
                charsRead += trimLength;
                text = trimmedText;
                return true;
            }

            static bool GetStringPart(ref ReadOnlySpan<char> text, ref int charsRead, out ReadOnlySpan<char> outerStringPart)
            {
                outerStringPart = default;
                if (
                    text.IsEmpty
                    || text[0] != Delimiter
                )
                {
                    return false;
                }
                var inner = text[1..];
                if (GetStringLength(inner) is not (> 0 and var length))
                {
                    return false;
                }
                outerStringPart = text[..(length + 2)];
                text = text[outerStringPart.Length..];
                charsRead += outerStringPart.Length;
                return true;
            }

            static int GetStringLength(ReadOnlySpan<char> text)
            {
                for (var i = 0; i < text.Length; ++i)
                {
                    switch (text[i])
                    {
                        case Delimiter:
                            return i;
                        case '\\' when i < text.Length - 1 && text[i + 1] == Delimiter:
                            ++i; // Skip the quote char
                            break;
                    }
                }
                return text.Length;
            }
        }

    };
    //tested
    public record WhitespaceToken(string Value) : Token(Value)
    {
        public bool HasNewline => this.Value.Contains('\n');

        public static bool TryConsume(string? text, [NotNullWhen(true)] out WhitespaceToken? value)
            => TryConsume(text, out var charsRead, out value) && charsRead == text?.Length;
        public static bool TryConsume(ReadOnlySpan<char> span, out int charsRead, [NotNullWhen(true)] out WhitespaceToken? value)
        {
            for (charsRead = 0; charsRead < span.Length; ++charsRead)
            {
                if (char.IsWhiteSpace(span[charsRead]) is false)
                {
                    break;
                }
            }
            if (charsRead is 0)
            {
                value = default;
                return false;
            }
            span = span[..charsRead];
            value = new WhitespaceToken(new string(span));
            return true;
        }

        public static WhitespaceToken Consume(ReadOnlySpan<char> span)
            => Consume(span, out _);
        public static WhitespaceToken Consume(ReadOnlySpan<char> span, out int charsRead)
            => TryConsume(span, out charsRead, out var token) ? token : new(string.Empty);

        public static WhitespaceToken Consume(TextReader reader)
        {
            var sb = new StringBuilder();
            while (char.IsWhiteSpace((char)reader.Peek()))
            {
                sb.Append(reader.ReadChar());
            }
            return new WhitespaceToken(sb.ToString());
        }
    };
}
