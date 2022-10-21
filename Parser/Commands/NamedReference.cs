namespace Parser
{
    public class NamedReference : ITable, IColumn
    {
        public string? Prefix { get; init; }
        public string? Name { get; init; }
        public string? Alias { get; init; }

        public int FulNameLength => (Prefix?.Length ?? 0) + (Name?.Length ?? 0) + 1;

        public string FullName => (Prefix == null ? string.Empty : Prefix + Constants.NameSeparator) + Name;

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
            else
            {
                Name = token.Value;
            }

            // maybe an alias?
            if (queue.TryPeek(out var aliasMaybe) && aliasMaybe.Value == Constants.AsKeyword)
            {
                queue.Dequeue(); // remove the keyword
                if (queue.TryPeek(out var v) && v is IdentifierToken)
                {
                    queue.Dequeue(); // remove the alias
                    Alias = v.Value;
                }
                else
                {
                    throw new InvalidDataException("Missing a valid identifier after an Alias");
                }
            }
        }

        public string Print(int indentSize, int indentCount)
        {
            var pad = string.Empty.PadLeft(indentSize * indentCount);
            return pad + PrintWithPaddedAlias(0);
        }

        public string PrintWithPaddedAlias(int size)
        {
            return (PrintNameAndPrefix().PadRight(size) + (Alias == null ? string.Empty : $" {Constants.AsKeyword} {Alias}")).TrimEnd();
        }

        public string PrintNameAndPrefix()
        {
            return (Prefix == null ? string.Empty : $"{Prefix}.") + Name;
        }

        public override string ToString()
        {
            return Print(0, 0);
        }
    }
}