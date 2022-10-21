namespace Parser
{
    public class Distinct : IClause
    {
        private readonly Columns? onColumns;

        public Distinct(Queue<Token> queue)
        {
            if (queue.TryPeek(out var distinctToken) && distinctToken.Value == Constants.DistinctKeyword)
            {
                queue.Dequeue();// remove the distinct keyword.
            }
            while (queue.TryPeek(out var token))
            {
                if (token is KeywordToken && token.Value == Constants.OnKeyword)
                {
                    queue.Dequeue(); // get rid of the on.

                    if (queue.TryPeek(out var open) && open is OperatorToken && open.Value == Constants.OpenParenthesis)
                    {
                        queue.Dequeue();
                    }

                    this.onColumns = new Columns(queue);

                    if (queue.TryPeek(out var close) && close is OperatorToken && close.Value == Constants.ClosingParenthesis)
                    {
                        queue.Dequeue();
                    }
                }
                return;
            }

            throw new ArgumentException("Unknown");
        }

        public string Print(int indentSize, int indentCount)
        {
            return $@" {Constants.DistinctKeyword}{MaybePrintOn(indentSize, indentCount)}";
        }

        private string MaybePrintOn(int indentSize, int indentCount)
        {
            if (onColumns == null)
            {
                return string.Empty;
            }
            var pad = string.Empty.PadLeft(indentSize * (indentCount + 1));
            return @$" {Constants.OnKeyword} {Constants.OpenParenthesis}
{onColumns.Print(indentSize, indentCount + 2)}
{pad}{Constants.ClosingParenthesis}";
        }
    }
}