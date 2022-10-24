namespace Parser
{
    public class NamedReference : INamedReference
    {
        public string? Prefix { get; init; }
        public string? Name { get; init; }

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
                    this.Name = col.Value;
                    this.Prefix = token.Value;
                }
                else
                {
                    throw new InvalidDataException("Missing a valid column after the table.");
                }
            }
            // col (without table signifier)
            else
            {
                this.Name = token.Value;
            }
        }

        public virtual string Print(int indentSize, int indentCount)
        {
            return (Prefix == null ? string.Empty : $"{Prefix}.") + Name;
        }
    }
}