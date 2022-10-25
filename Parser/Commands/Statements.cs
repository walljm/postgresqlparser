namespace Parser
{
    public abstract class Statement : IItem
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
                    .TakeWhile(o => !(o is OperatorToken && o.Value == Constants.CommandTerminator)).ToList();

                if (statementTokens.Count == 0)
                {
                    break;
                }

                i += statementTokens.Count;
                expressions.Add(ParseStatement(statementTokens));
            }
            return expressions;
        }

        public static IItem ParseStatement(IList<Token> statementTokens)
        {
            var queue = new Queue<Token>(statementTokens);
            if (!queue.TryPeek(out var first))
            {
                throw new ArgumentException("Must have at least 1 token.");
            }

            switch (first)
            {
                case KeywordToken token:
                    {
                        if (token.Value == Constants.SelectKeyword && Select.TryParse(queue, out var select))
                        {
                            return select ?? throw new InvalidOperationException("Null when true");
                        }
                    }
                    break;
            }

            throw new NotSupportedException();
        }

        public abstract string Print(int indentSize, int indentCount);
    }
}