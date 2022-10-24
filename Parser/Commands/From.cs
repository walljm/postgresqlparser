namespace Parser
{
    public class From : IClause
    {
        private readonly ITable table;

        public From(Queue<Token> queue)
        {
            while (queue.TryDequeue(out var token))
            {
                if (token is IdentifierToken)
                {
                    this.table = new AliasedNamedReference(queue, token);
                    return; // exit the loop and finish the constructor.
                }
                else if (token is OperatorToken && token.Value == Constants.OpenParenthesis)
                {
                    this.table = new SubSelect(queue);
                }
            }

            throw new ArgumentException("Unknown table");
        }

        public string Print(int indentSize, int indentCount)
        {
            var indent = indentSize * indentCount;
            var pad = string.Empty.PadLeft(indent);
            return $@"{pad}{Constants.FromKeyword}
{this.table.Print(indentSize, indentCount + 1)}";
        }
    }
}