namespace Parser
{
    public class Columns : IClause
    {
        private readonly List<IColumn> columns = new();

        public Columns(Queue<Token> queue)
        {
            while (queue.TryDequeue(out var token))
            {
                if (token is IdentifierToken || (token is OperatorToken && token.Value == Constants.AsterixSeparator))
                {
                    this.columns.Add(new NamedReference(queue, token));

                    if (queue.TryPeek(out var nextColumn) && nextColumn.Value == Constants.CommaSeparator)
                    {
                        queue.Dequeue(); // remove the next item indicator so you're ready to check the next thing.
                        continue; // you have more columns to process
                    }
                    else
                    {
                        break; // exit the loop and finish the constructor.
                    }
                }
            }
        }

        public string Print(int indentSize, int indentCount)
        {
            var pad = string.Empty.PadRight(indentSize * indentCount);
            var max = this.columns.Max(o => o.FulNameLength);

            return @$"{pad} {string.Join(Environment.NewLine + pad + ",", columns.Select(o => o.PrintWithPaddedAlias(max)))}";
        }
    }
}