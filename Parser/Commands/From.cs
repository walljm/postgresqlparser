namespace Parser
{
    public class From : IClause
    {
        public readonly ITable Table;

        public From(ITable table)
        {
            this.Table = table;
        }

        public static bool TryParse(Queue<Token> queue, out From? table)
        {
            table = null;
            if (queue.TryPeek(out var test) && test is KeywordToken && test.Value == Constants.FromKeyword)
            {
                queue.Dequeue();

                if (queue.TryPeek(out var identToken) && identToken is IdentifierToken)
                {
                    if (AliasedNamedReference.TryParse(queue, out var aliasedNamedReference))
                    {
                        table = new From(aliasedNamedReference ?? throw new InvalidOperationException("Null returned when try parse was true."));
                    }
                    else
                    {
                        throw new InvalidOperationException("Expected a table reference");
                    }

                    return true; // exit the loop and finish the constructor.
                }
                else if (queue.TryPeek(out var parenToken) && parenToken is OperatorToken && parenToken.Value == Constants.OpenParenthesis)
                {
                    if (SubSelect.TryParse(queue, out var subSelect))
                    {
                        table = new From(subSelect ?? throw new InvalidOperationException("Null returned when try parse was true."));
                    }
                    else
                    {
                        throw new InvalidOperationException("Expected a table reference");
                    }

                    return true;
                }
            }

            return false;
        }

        public string Print(int indentSize, int indentCount)
        {
             var indent = indentSize * indentCount;
            var pad = string.Empty.PadLeft(indent);
            return $@"{pad}{Constants.FromKeyword}
{this.Table.Print(indentSize, indentCount + 1)}";
        }
    }
}