using System.Text;

namespace Parser
{
    public sealed partial class Tokenizer : IDisposable
    {
        private readonly TextReader reader;

        public Tokenizer(TextReader reader)
        {
            this.reader = reader;
        }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize

        public void Dispose() => ((IDisposable)reader).Dispose();

#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize

        public IList<Token> Scan()
        {
            var tokens = new List<Token>();
            while (reader.Peek() != -1)
            {
                // these must be done in this order...

                if (WhitespaceToken.TryConsume(reader, out var whitespaceToken))
                {
                    tokens.Add(whitespaceToken);
                    continue;
                }
                else if (NumericToken.TryConsume(reader, out var numericToken))
                {
                    tokens.Add(numericToken);
                    continue;
                }
                else if (OperatorToken.TryConsume(reader, out var operatorToken))
                {
                    tokens.Add(operatorToken);
                    continue;
                }
                else if (IdentifierToken.TryConsume(reader, out var identifierToken))
                {
                    tokens.Add(identifierToken);
                    continue;
                }
                else if (StringLiteralToken.TryConsume(reader, out var stringLiteralToken))
                {
                    tokens.Add(stringLiteralToken);
                    continue;
                }
                throw new Exception("Unknown character in expression: " + reader.Peek());
            }

            return tokens;
        }
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
            return char.IsDigit(c)
                || c == '.'
                || c == 'e'
                || c == '-'
                || c == '+'
                ;
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
        public static bool TryConsume(TextReader reader, out NumericToken? value)
        {
            if (!reader.CanRead() || !IsValidStartChar(reader.Peek()))
            {
                value = null;
                return false;
            }

            var sb = new StringBuilder();
            sb.Append(reader.ReadChar());

            while (reader.CanRead() && IsValidChar(reader.Peek()))
            {
                sb.Append(reader.ReadChar());
            }

            value = new NumericToken(sb.ToString());
            return true;
        }
    };
    //tested
    public record OperatorToken(string Value) : Token(Value)
    {
        private static readonly HashSet<char> nextOperators = new(new char[]
        {
            // boolean
            '<', '>', '=',
            ':',']','[',
        });

        private static readonly HashSet<char> operators = new(new char[]
        {
            // boolean
            '<', '>', '=',
            // arithmatic
            '+', '-', '/','*',
            // delimiters
            '~', '!', '@', '#', '%', '^', '&', '|', '?', '`',
        });
        private static readonly HashSet<char> specialChars = new(new char[]
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
        public static bool TryConsume(TextReader reader, out OperatorToken? value)
        {
            if (!reader.CanRead() || !IsOperatorChar(reader.Peek()))
            {
                value = null;
                return false;
            }

            var sb = new StringBuilder();
            sb.Append(reader.ReadChar());

            while (reader.CanRead() && IsNextOperatorChar(reader.Peek()))
            {
                sb.Append((char)reader.Read());
            }

            value = new OperatorToken(sb.ToString());
            return true;
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

        public static bool TryConsume(TextReader reader, out IdentifierToken? value)
        {
            if (TryConsumeQuoted(reader, out var quotedIdentifier))
            {
                value = quotedIdentifier;
                return true;
            }

            var c = (char)reader.Peek();
            if (!IsIdentifierBeginningChar(c))
            {
                value = null;
                return false;
            }

            var sb = new StringBuilder();
            while (IsIdentifierChar((char)reader.Peek()))
            {
                sb.Append((char)reader.Read());
            }
            var v = sb.ToString().ToUpper();

            value = Tokenizer.Keywords.Contains(v)
                ? new KeywordToken(v)
                : new IdentifierToken(sb.ToString());
            return true;
        }

        // TODO: someday, lets support the U&"" unicode variation.
        // TODO: should we allow escaped quotes in this?
        private static bool TryConsumeQuoted(TextReader reader, out QuotedIdentifier? value)
        {
            if (!reader.CanRead() || reader.PeekChar() != QuotedDelimiter)
            {
                value = null;
                return false;
            }

            value = new QuotedIdentifier(reader.ReadWrappedString(QuotedDelimiter));
            return true;
        }
    };
    //tested
    public record KeywordToken(string Value) : IdentifierToken(Value)
    {
    };
    //tested
    public record QuotedIdentifier(string Value) : IdentifierToken(Value)
    {
    };
    //tested
    public record StringLiteralToken(string Value) : Token(Value)
    {
        public static readonly char Delimiter = '\'';

        // TODO: handle '' escaping
        // TODO: you should support the E'' escaped string format at some point.
        public static bool TryConsume(TextReader reader, out StringLiteralToken? value)
        {
            if (!reader.CanRead() || (char)reader.Peek() != Delimiter)
            {
                value = null;
                return false;
            }

            var sb = new StringBuilder(); // initialize with the tick
            sb.Append(reader.ReadChar());
            // if its not the ' and its not escaped.
            while (reader.CanRead() && (reader.PeekChar() != Delimiter || sb.FromEnd(1) == '\\'))
            {
                sb.Append(reader.ReadChar());
            }

            if (!reader.CanRead())
            {
                throw new InvalidOperationException("Premature end of string literal");
            }

            var maybeClosing = (char)reader.Read(); // append the closeing delimiter.
            var ws = WhitespaceToken.Consume(reader); // consume any whitespace.
            if (ws.HasWhitespace && ws.HasNewline && (char)reader.Peek() == Delimiter)
            {
                // according to postgres spec, we ignore two ticks with a new line between them.
                //  i.e. 'foo' \n 'bar' becomes 'foobar'
                if (TryConsume(reader, out value))
                {
                    sb.Append(value?.Value?.Substring(1)); // consume the next literal.
                }
            }
            else
            {
                // its not a new line delimited string, so add the closing tick.
                sb.Append(maybeClosing);
            }

            value = new StringLiteralToken(sb.ToString());
            return true;
        }
    };
    //tested
    public record WhitespaceToken(bool HasNewline, bool HasWhitespace, string Value) : Token(Value)
    {
        public static bool TryConsume(TextReader reader, out WhitespaceToken? value)
        {
            if (!reader.CanRead() || !char.IsWhiteSpace(reader.PeekChar()))
            {
                value = new WhitespaceToken(false, false, string.Empty);
                return false;
            }

            value = Consume(reader);
            return true;
        }

        public static WhitespaceToken Consume(TextReader reader)
        {
            bool hasNewline = false;
            bool hasWhitespace = false;
            var sb = new StringBuilder();
            while (char.IsWhiteSpace((char)reader.Peek()))
            {
                hasWhitespace = true;
                var c = reader.ReadChar(); // throw away the whitespace.
                if (c == '\n')
                {
                    hasNewline = true;
                }
                sb.Append((char)c);
            }
            return new WhitespaceToken(hasNewline, hasWhitespace, sb.ToString());
        }
    };
}