using System.Diagnostics.CodeAnalysis;

namespace Parser
{
    public class SubSelect : ITable
    {
        public Select Select { get; init; }

        public SubSelect(Select select)
        {
            this.Select = select;
        }

        // TODO: Because of the ambiguity noted w/ Select.TryParse, I did not put [NotNullWhen(true)] on the SubSelect attribute
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
                // TODO: What do we do if Select.TryParse fails?  We can't backtrack the token queue.
                // Throwing an exception isn't really appropriate in a 'Try' method.
                select = new SubSelect(innerSelect);
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
