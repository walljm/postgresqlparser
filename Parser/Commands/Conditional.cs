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

        public static bool TryParse(Queue<Token> queue, out Conditional? conditional)
        {
            conditional = null;


            if (queue.TryPeek(out var token) && token is IdentifierToken)
            {
                NamedReference left;
                string op;
                NamedReference right;
                if (NamedReference.TryParse(queue, out var leftNamedReference))
                {
                    left = leftNamedReference ?? throw new InvalidOperationException("Null returned when try parse was true.");
                }
                else
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

                if (NamedReference.TryParse(queue, out var rightNamedReference))
                {
                    right = rightNamedReference ?? throw new InvalidOperationException("Null returned when try parse was true.");
                }
                else
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