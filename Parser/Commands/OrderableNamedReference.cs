namespace Parser
{
    public class OrderableNamedReference : NamedReference, IPaddedReference
    {
        public string? Direction { get; init; } = Constants.AscendingKeyword;

        public OrderableNamedReference(string? prefix, string? name, string? direction) : base(prefix, name)
        {
            this.Direction = direction;
        }

        public OrderableNamedReference(string? prefix, string? name) : base(prefix, name)
        {
        }

        public static bool TryParse(Queue<Token> queue, out OrderableNamedReference? orderableNamedReference)
        {
            orderableNamedReference = null;

            if (NamedReference.TryParse(queue, out var namedReference))
            {
                // maybe an sort direction?
                if (queue.TryPeek(out var sortMaybe) && (sortMaybe.Value == Constants.AscendingKeyword || sortMaybe.Value == Constants.DescendingKeyword))
                {
                    queue.Dequeue(); // remove the keyword
                    orderableNamedReference = new OrderableNamedReference(namedReference?.Prefix, namedReference?.Name, sortMaybe.Value);
                    return true;
                }
                else
                {
                    orderableNamedReference = new OrderableNamedReference(namedReference?.Prefix, namedReference?.Name);
                    return true;
                }
            }

            return false;
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