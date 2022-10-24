namespace Parser
{
    public class GroupBy : IClause
    {
        public List<INamedReference> Columns { get; init; }

        public GroupBy(List<INamedReference> columns)
        {
            this.Columns = columns;
        }

        public static bool TryParse(Queue<Token> queue, out GroupBy? orderBy)
        {
            orderBy = null;

            if (queue.TryPeek(out var test) && test is KeywordToken && test.Value == Constants.GroupKeyword)
            {
                queue.Dequeue();

                if (!queue.TryPeek(out var bykeyword2) || bykeyword2.Value != Constants.ByKeyword)
                {
                    throw new InvalidDataException($"{Constants.GroupKeyword} must be followed by '{Constants.ByKeyword}' keyword.");
                }

                queue.Dequeue();

                List<INamedReference> columns = new();
                while (queue.TryPeek(out var token) && token is IdentifierToken)
                {
                    if (AliasedNamedReference.TryParse(queue, out var aliasedNamedReference))
                    {
                        columns.Add(aliasedNamedReference ?? throw new InvalidOperationException("Null returned when try parse was true."));
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
                orderBy = new GroupBy(columns);
                return true;
            }
            return false;
        }

        public string Print(int indentSize, int indentCount)
        {
            var pad = string.Empty.PadRight(indentSize * indentCount);
            var innerPad = string.Empty.PadRight(indentSize * (indentCount + 1));

            return @$"{pad}{Constants.GroupKeyword} {Constants.ByKeyword}
{innerPad} {string.Join(Environment.NewLine + innerPad + ",", Columns.Select(o => o.Print(indentSize, indentCount)))}";
        }
    }
}