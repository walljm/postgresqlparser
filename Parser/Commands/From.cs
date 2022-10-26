using System.Diagnostics.CodeAnalysis;

namespace Parser
{
    public class From : IClause
    {
        public readonly ITable Table;

        public From(ITable table)
        {
            this.Table = table;
        }


        public static bool TryParse(ref Tokenizer tokenizer, [NotNullWhen(true)] out From? from)
        {
            throw new NotImplementedException();
        }
        public static bool TryParse(Queue<Token> queue, out From? table)
        {
            table = null;
            if (!queue.TryPeek(out var test) || test is not KeywordToken { Value: Constants.FromKeyword })
            {
                return false;
            }
            queue.Dequeue();
            if (queue.TryPeek(out var identToken) && identToken is IdentifierToken)
            {
                if (AliasedNamedReference.TryParse(queue, out var aliasedNamedReference))
                {
                    table = new From(aliasedNamedReference);
                }
                else
                {
                    throw new InvalidOperationException("Expected a table reference");
                }
                return true; // exit the loop and finish the constructor.
            }

            if (!queue.TryPeek(out var parenToken) || parenToken is not OperatorToken { Value: Constants.OpenParenthesis })
            {
                return false;
            }
            if (!SubSelect.TryParse(queue, out var subSelect) || subSelect is null)
            {
                throw new InvalidOperationException("Expected a table reference");
            }
            table = new From(subSelect);
            return true;

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
