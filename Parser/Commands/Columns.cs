using System.Diagnostics.CodeAnalysis;

namespace Parser
{
    public class Columns : IClause
    {
        public List<IPaddedReference> ColumnList { get; init; }

        public Columns(List<IPaddedReference> columns)
        {
            this.ColumnList = columns;
        }

        public static bool TryParse(ref Tokenizer tokenizer, [NotNullWhen(true)] out Columns? columns)
        {
            throw new NotImplementedException();
        }
        public static bool TryParse(Queue<Token> queue, [NotNullWhen(true)] out Columns? columns)
        {
            var cols = new List<IPaddedReference>();
            while (queue.TryPeek(out var token) && token is IdentifierToken || (token is OperatorToken && token.Value == Constants.AsterixSeparator))
            {
                if (AliasedNamedReference.TryParse(queue, out var aliasedNamedReference))
                {
                    cols.Add(aliasedNamedReference);
                }
                else
                {
                    throw new InvalidOperationException("Expected a column reference");
                }

                if (queue.TryPeek(out var nextColumn) && nextColumn.Value == Constants.CommaSeparator)
                {
                    queue.Dequeue(); // remove the next item indicator so you're ready to check the next thing.
                    continue; // you have more columns to process
                }
                columns = new Columns(cols);
                return true; // exit the loop
            }

            columns = null;
            return false;
        }

        public string Print(int indentSize, int indentCount)
        {
            var pad = string.Empty.PadRight(indentSize * indentCount);
            var max = this.ColumnList.Max(o => o.FulNameLength);

            return @$"{pad} {string.Join(Environment.NewLine + pad + ",", ColumnList.Select(o => o.PrintPadded(max)))}";
        }

    }
}
