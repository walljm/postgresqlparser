namespace Parser
{
    public class OrderBy : IClause
    {
        public List<IPaddedReference> Columns { get; init; }

        public OrderBy(List<IPaddedReference> columns)
        {
            this.Columns = columns;
        }

        public static bool TryParse(Queue<Token> queue, out OrderBy? orderBy)
        {
            orderBy = null;

            if (queue.TryPeek(out var test) && test is KeywordToken && test.Value == Constants.OrderKeyword)
            {
                queue.Dequeue();

                if (!queue.TryPeek(out var bykeyword2) || bykeyword2.Value != Constants.ByKeyword)
                {
                    throw new InvalidDataException($"{Constants.OrderKeyword} must be followed by '{Constants.ByKeyword}' keyword.");
                }

                queue.Dequeue();

                List<IPaddedReference> columns = new();
                while (queue.TryPeek(out var token) && token is IdentifierToken)
                {
                    if (OrderableNamedReference.TryParse(queue, out var orderableNamedReference))
                    {
                        columns.Add(orderableNamedReference ?? throw new InvalidOperationException("Null returned when try parse was true."));
                    }
                    else
                    {
                        throw new InvalidOperationException("Expected a table reference");
                    }

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
                orderBy = new OrderBy(columns);
                return true;
            }
            return false;
        }

        public string Print(int indentSize, int indentCount)
        {
            var pad = string.Empty.PadRight(indentSize * indentCount);
            var innerPad = string.Empty.PadRight(indentSize * (indentCount + 1));
            var max = this.Columns.Max(o => o.FulNameLength);

            return @$"{pad}{Constants.OrderKeyword} {Constants.ByKeyword}
{innerPad} {string.Join(Environment.NewLine + innerPad + ",", Columns.Select(o => o.PrintPadded(max)))}";
        }
    }
}