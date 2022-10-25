namespace Parser
{
    public static class Statement
    {
        public static IEnumerable<IItem> Parse(IEnumerable<Token> rawtokens)
        {
            var tokens = rawtokens.Where(o => o is not WhitespaceToken).ToArray();

            var expressions = new List<IItem>();
            // convert tokens into statements.
            //  a statement is a list of tokens that ends with a semicolon ';'
            for (var i = 0; i < tokens.Length; i++)
            {
                var statementTokens = tokens
                    .Skip(i)
                    .SkipWhile(o => o is WhitespaceToken)
                    .TakeWhile(o => !(o is OperatorToken && o.Value == Constants.CommandTerminator)).ToList();

                if (statementTokens.Count == 0)
                {
                    break;
                }

                i += statementTokens.Count;
                var queue = new Queue<Token>(statementTokens.Where(o => o is not WhitespaceToken));
                expressions.Add(ParseStatement(queue));
            }
            return expressions;
        }

        private static IItem ParseStatement(Queue<Token> queue)
        {
            if (!queue.TryPeek(out var first))
            {
                throw new ArgumentException("Must have at least 1 token.");
            }

            switch (first)
            {
                case KeywordToken token when token.Value == Constants.SelectKeyword:
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