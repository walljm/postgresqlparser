using System.Diagnostics.CodeAnalysis;

namespace Parser
{
    public class AliasedNamedReference : NamedReference, ITable, IPaddedReference
    {
        public string? Alias { get; init; }

        public AliasedNamedReference(string? prefix, string? name, string? alias) : base(prefix, name)
        {
            this.Alias = alias;
        }

        public static bool TryParse(Queue<Token> queue, [NotNullWhen(true)] out AliasedNamedReference? aliasedNamedReference)
        {
            aliasedNamedReference = null;

            if (NamedReference.TryParse(queue, out var namedReference))
            {
                // maybe an sort direction?
                if (queue.TryPeek(out var aliasMaybe) && aliasMaybe.Value == Constants.AsKeyword)
                {
                    queue.Dequeue(); // remove the keyword
                    var alias = queue.Dequeue(); // remove the value
                    aliasedNamedReference = new AliasedNamedReference(namedReference?.Prefix, namedReference?.Name, alias.Value);
                    return true;
                }
                else
                {
                    aliasedNamedReference = new AliasedNamedReference(namedReference?.Prefix, namedReference?.Name, null);
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
