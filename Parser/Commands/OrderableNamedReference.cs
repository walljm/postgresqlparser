namespace Parser
{
    public class OrderableNamedReference : NamedReference, IPaddedReference
    {
        public string? Direction { get; init; } = Constants.AscendingKeyword;

        public OrderableNamedReference(Queue<Token> queue, Token token) : base(queue, token)
        {
            // maybe an sort direction?
            if (queue.TryPeek(out var sortMaybe) && (sortMaybe.Value == Constants.AscendingKeyword || sortMaybe.Value == Constants.DescendingKeyword))
            {
                queue.Dequeue(); // remove the keyword
                Direction = sortMaybe.Value;
            }
        }

        public override string Print(int indentSize, int indentCount)
        {
            var pad = string.Empty.PadLeft(indentSize * indentCount);
            return pad + PrintPadded(0);
        }

        public string PrintPadded(int size)
        {
            return (PrintNameAndPrefix().PadRight(size) + (Direction == null ? string.Empty : $" {Direction}")).TrimEnd();
        }

        protected string PrintNameAndPrefix()
        {
            return (Prefix == null ? string.Empty : $"{Prefix}.") + Name;
        }
    }
}