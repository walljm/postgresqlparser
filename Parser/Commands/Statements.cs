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

            return first switch
            {
                KeywordToken token => token.Value switch
                {
                    Constants.SelectKeyword => new Select(queue),
                    _ => throw new NotSupportedException(),
                },
                _ => throw new NotSupportedException(),
            };
        }

        public abstract string Print(int indentSize, int indentCount);
    }
}