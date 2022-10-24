namespace Parser
{
    public class AliasedNamedReference : NamedReference, ITable, IPaddedReference
    {
        public string? Alias { get; init; }

        public AliasedNamedReference(Queue<Token> queue, Token token) : base(queue, token)
        {
            // maybe an alias?
            if (queue.TryPeek(out var aliasMaybe) && aliasMaybe.Value == Constants.AsKeyword)
            {
                queue.Dequeue(); // remove the keyword
                if (queue.TryPeek(out var v) && v is IdentifierToken)
                {
                    queue.Dequeue(); // remove the alias
                    this.Alias = v.Value;
                }
                else
                {
                    throw new InvalidDataException("Missing a valid identifier after an Alias");
                }
            }
        }

        public override string Print(int indentSize, int indentCount)
        {
            var pad = string.Empty.PadLeft(indentSize * indentCount);
            return pad + PrintPadded(0);
        }

        public string PrintPadded(int size)
        {
            return (PrintNameAndPrefix().PadRight(size) + (Alias == null ? string.Empty : $" {Constants.AsKeyword} {Alias}")).TrimEnd();
        }

        protected string PrintNameAndPrefix()
        {
            return (Prefix == null ? string.Empty : $"{Prefix}.") + Name;
        }

        public override string ToString()
        {
            return Print(0, 0);
        }
    }
}