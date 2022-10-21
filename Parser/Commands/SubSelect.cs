namespace Parser
{
    public class SubSelect : ITable
    {
        private readonly Select select;

        public SubSelect(Queue<Token> queue)
        {
            if (queue.TryPeek(out var token) && token is OperatorToken && token.Value == Constants.OpenParenthesis)
            {
                queue.Dequeue();
            }
            else
            {
                throw new ArgumentException("Missing opening parenthesis!");
            }
            this.select = new Select(queue);
            if (queue.TryPeek(out var close) && close is OperatorToken && close.Value == Constants.ClosingParenthesis)
            {
                queue.Dequeue();
            }
            else
            {
                throw new ArgumentException("Missing closing parenthesis!");
            }
        }

        public string Print(int indentSize, int indentCount)
        {
            var indent = indentSize * indentCount;
            var pad = string.Empty.PadLeft(indent);
            return $@"{pad}(
{this.select.Print(indentSize, indentCount + 1)}
{pad})";
        }
    }
}