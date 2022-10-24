namespace Parser
{
    public class Distinct : IClause
    {
        public Columns? OnColumns { get; init; }

        public Distinct(Columns columns)
        {
            OnColumns = columns;
        }

        public Distinct()
        {
        }

        public static bool TryParse(Queue<Token> queue, out Distinct? distinct)
        {
            distinct = null;
            if (!queue.TryPeek(out var distinctToken) || distinctToken.Value != Constants.DistinctKeyword)
            {
                return false;
            }
            queue.Dequeue();// remove the distinct keyword.

            // ON is optional.
            if (queue.TryPeek(out var token) && token is KeywordToken && token.Value == Constants.OnKeyword)
            {
                queue.Dequeue(); // get rid of the on.

                if (queue.TryPeek(out var open) && open is OperatorToken && open.Value == Constants.OpenParenthesis)
                {
                    queue.Dequeue();
                }

                if (Columns.TryParse(queue, out var columns))
                {
                    distinct = new Distinct(columns ?? throw new InvalidOperationException("Null returned when try parse was true."));
                }
                else
                {
                    throw new InvalidOperationException("No columns provided in ON clause");
                }

                if (queue.TryPeek(out var close) && close is OperatorToken && close.Value == Constants.ClosingParenthesis)
                {
                    queue.Dequeue();
                }
            }
            else
            {
                distinct = new Distinct();
            }

            return true;
        }

        public string Print(int indentSize, int indentCount)
        {
            return $@" {Constants.DistinctKeyword}{MaybePrintOn(indentSize, indentCount)}";
        }

        private string MaybePrintOn(int indentSize, int indentCount)
        {
            if (OnColumns == null)
            {
                return string.Empty;
            }
            var pad = string.Empty.PadLeft(indentSize * (indentCount + 1));
            return @$" {Constants.OnKeyword} {Constants.OpenParenthesis}
{OnColumns.Print(indentSize, indentCount + 2)}
{pad}{Constants.ClosingParenthesis}";
        }
    }
}