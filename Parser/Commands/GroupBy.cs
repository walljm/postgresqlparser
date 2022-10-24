namespace Parser
{
    public class GroupBy : IClause
    {
        private readonly List<INamedReference> columns = new();

        public GroupBy(Queue<Token> queue)
        {
            while (queue.TryDequeue(out var token))
            {
                if (token is IdentifierToken)
                {
                    this.columns.Add(new AliasedNamedReference(queue, token));

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
            var innerPad = string.Empty.PadRight(indentSize * (indentCount + 1));

            return @$"{pad}{Constants.GroupKeyword} {Constants.ByKeyword}
{innerPad} {string.Join(Environment.NewLine + innerPad + ",", columns.Select(o => o.Print(indentSize, indentCount)))}";
        }
    }
}