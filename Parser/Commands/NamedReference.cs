namespace Parser
{
    public class NamedReference : ITable
    {
        public string? Name { get; init; }
        public string? Prefix { get; init; }
        public string? Alias { get; init; }

        public NamedReference(Queue<Token> queue, Token token)
        {
            // tbl.col
            if (queue.TryPeek(out var next) && next.Value == Constants.NameSeparator)
            {
                queue.Dequeue(); // remove the name separator.
                if (queue.TryPeek(out var col) && next.Value == Constants.NameSeparator)
                {
                    queue.Dequeue(); // remove the column
                    Name = col.Value;
                    Prefix = token.Value;
                }
                else
                {
                    throw new InvalidDataException("Missing a valid column after the table.");
                }
            }
            // col (without table signifier)
            else if (queue.TryPeek(out var comma) && (comma.Value == Constants.CommaSeparator || comma.Value == Constants.AsKeyword))
            {
                Name = token.Value;
            }

            // maybe an alias?
            if (queue.TryPeek(out var aliasMaybe) && aliasMaybe.Value == Constants.AsKeyword)
            {
                queue.Dequeue(); // remove the AS keyword
                if (queue.TryPeek(out var v) && v is IdentifierToken)
                {
                    queue.Dequeue();
                    Alias = v.Value;
                }
                else
                {
                    throw new InvalidDataException("Missing a valid identifier after an Alias");
                }
            }
        }

        public string Print()
        {
            return (Prefix == null ? string.Empty : $"{Prefix}.") + Name + (Alias == null ? string.Empty : $" AS {Alias}");
        }

        public override string ToString()
        {
            return Print();
        }
    }
}