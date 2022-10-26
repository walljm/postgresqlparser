using System.Diagnostics.CodeAnalysis;

namespace Parser
{
    public class Conditional : IItem
    {
        public NamedReference Left { get; init; }
        public string? Operator { get; set; }
        public NamedReference? Right { get; init; }

        public string FullExpresion => $"{Left.Print(0, 0)} {(Operator ?? string.Empty)} {(Right == null ? string.Empty : Right?.Print(0, 0))}";

        public Conditional(NamedReference left, string operatorValue, NamedReference right)
        {
            this.Left = left;
            this.Operator = operatorValue;
            this.Right = right;
        }

        public Conditional(NamedReference left)
        {
            this.Left = left;
        }

        public static bool TryParse(Queue<Token> queue, [NotNullWhen(true)] out Conditional? conditional)
        {
            conditional = null;


            if (queue.TryPeek(out var token) && token is IdentifierToken)
            {
                string op;
                if (!NamedReference.TryParse(queue, out var left))
                {
                    throw new InvalidOperationException("Expected a column reference");
                }
                if (queue.TryPeek(out var opToken) && opToken is OperatorToken)
                {
                    queue.Dequeue();
                    op = opToken.Value;
                }
                else
                {
                    // this is a case where a bool value or column reference is used without a comparator.
                    conditional = new Conditional(left);
                    return true;
                }

                if (!NamedReference.TryParse(queue, out var right))
                {
                    throw new InvalidOperationException("Expected a column reference");
                }
                conditional = new Conditional(left, op, right);
                return true; // exit the loop
            }

            conditional = null;
            return false;
        }

        public string Print(int indentSize, int indentCount)
        {
            var pad = string.Empty.PadRight(indentSize * indentCount);

            return @$"{pad}{FullExpresion}";
        }
    }
}
