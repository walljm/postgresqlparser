namespace Parser
{
    public class SubSelect : ITable
    {
        public Select Select { get; init; }

        public SubSelect(Select select)
        {
            this.Select = select;
        }

        public static bool TryParse(Queue<Token> queue, out SubSelect? select)
        {
            select = null;

            if (queue.TryPeek(out var token) && token is OperatorToken && token.Value == Constants.OpenParenthesis)
            {
                queue.Dequeue();
            }
            else
            {
                return false;
            }

            if (Select.TryParse(queue, out var innerSelect))
            {
                select = new SubSelect(innerSelect ?? throw new InvalidOperationException("Null found after successful parse"));
            }

            if (queue.TryPeek(out var close) && close is OperatorToken && close.Value == Constants.ClosingParenthesis)
            {
                queue.Dequeue();
            }
            else
            {
                throw new ArgumentException("Missing closing parenthesis!");
            }

            return true;
        }

        public string Print(int indentSize, int indentCount)
        {
            var indent = indentSize * indentCount;
            var pad = string.Empty.PadLeft(indent);
            return $@"{pad}(
{this.Select.Print(indentSize, indentCount + 1)}
{pad})";
        }
    }
}