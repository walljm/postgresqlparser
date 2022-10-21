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
                    this.table = new NamedReference(queue, token);
                    return; // exit the loop and finish the constructor.
                }
                else if (token is OperatorToken && token.Value == Constants.OpenParenthesis)
                {
                    this.table = new SubSelect(queue);
                }
            }

            throw new ArgumentException("Unknown table");
        }

        public string Print(int indent)
        {
            return this.table.Print(indent);
        }
    }
}