using System.Diagnostics.CodeAnalysis;

namespace Parser
{
    public class NamedReference : INamedReference
    {
        public string? Prefix { get; init; }
        public string? Name { get; init; }

        public string FullName => (Prefix == null ? string.Empty : Prefix + Constants.NameSeparator) + Name;

        public NamedReference(string? prefix, string? name)
        {
            this.Prefix = prefix;
            this.Name = name;
        }

        public static bool TryParse(Queue<Token> queue, [NotNullWhen(true)] out NamedReference? namedReference)
        {
            namedReference = null;
            if (queue.TryPeek(out var token) && token is IdentifierToken || (token is OperatorToken && token.Value == Constants.AsterixSeparator))
            {
                queue.Dequeue(); // remove the prefix or name

                // tbl.col
                if (queue.TryPeek(out var next) && next.Value == Constants.NameSeparator)
                {
                    queue.Dequeue(); // remove the name separator.
                    if (queue.TryPeek(out var col) && next.Value == Constants.NameSeparator)
                    {
                        queue.Dequeue(); // remove the column
                        namedReference = new NamedReference(token.Value, col.Value);
                    }
                    else
                    {
                        return false;
                    }
                }
                // col (without table signifier)
                else
                {
                    namedReference = new NamedReference(null, token.Value);
                }

                return true;
            }
            return false;
        }

        public virtual string Print(int indentSize, int indentCount)
        {
            return (Prefix == null ? string.Empty : $"{Prefix}.") + Name;
        }
    }
}
