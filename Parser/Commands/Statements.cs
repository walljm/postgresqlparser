using System.Diagnostics.CodeAnalysis;

namespace Parser
{
    public static class Statement
    {
        public static IEnumerable<IItem> Parse(IEnumerable<Token> rawtokens)
        {
            var tokens = rawtokens.Where(o => o is not WhitespaceToken).ToArray();
            var segment = new ArraySegment<Token>(tokens);
            foreach (var statementRange in GetStatementSegments(tokens))
            {
                var statementSegment = segment[statementRange];
                yield return ParseStatement(new Queue<Token>(statementSegment));
            }
            static IEnumerable<Range> GetStatementSegments(IReadOnlyList<Token> tokens)
            {
                var startIndex = 0;
                while (startIndex < tokens.Count)
                {
                    for (var endIndex = startIndex; endIndex < tokens.Count; ++endIndex)
                    {
                        if (tokens[endIndex] is not OperatorToken { Value: Constants.CommandTerminator })
                        {
                            continue;
                        }
                        yield return new(startIndex, endIndex);
                        startIndex = endIndex + 1;
                        break;
                    }
                }
            }
        }

        private static bool TryParseStatement(ref Tokenizer tokenizer, [NotNullWhen(true)] out IItem? result)
        {
            result = default;
            return tokenizer.PeekToken() switch
            {
                KeywordToken { Value: Constants.SelectKeyword }
                    => Select.TryParse(ref tokenizer, out var r) && Out(r, out result),
                KeywordToken { Value: var keywordValue }
                    => throw new NotSupportedException($"Unsupported keyword {keywordValue}"),
                _ => false,
            };

            static bool Out<TIn, TOut>(TIn? input, [NotNullWhen(true)] out TOut? output)
                where TIn : TOut
            {
                output = input;
                return output is not null;
            }
        }

        private static IItem ParseStatement(Queue<Token> queue)
        {
            if (!queue.TryPeek(out var first))
            {
                throw new ArgumentException("Must have at least 1 token.");
            }

            switch (first)
            {
                case KeywordToken { Value: Constants.SelectKeyword }:
                    {
                        if (Select.TryParse(queue, out var select))
                        {
                            return select ?? throw new InvalidOperationException("Null when true");
                        }
                        throw new InvalidOperationException("Unable to parse SELECT!");
                    }
            }

            throw new NotSupportedException("Unsupported statement!");
        }
    }
}
