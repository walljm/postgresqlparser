namespace Parser
{
    public class Limit : IClause
    {
        public string? Size { get; init; }

        public Limit(string? size)
        {
            Size = size;
        }

        public static bool TryParse(Queue<Token> queue, out Limit? limit)
        {
            limit = null;
            if (queue.TryPeek(out var token) && token.Value == Constants.LimitKeyword)
            {
                queue.Dequeue();
                if (queue.TryPeek(out var size) && size is NumericToken)
                {
                    queue.Dequeue();
                    limit = new Limit(size.Value);
                    return true;
                }
                else
                {
                    throw new ArgumentException($"Invalid {Constants.LimitKeyword} value.");
                }
            }
            return false;
        }
        public string Print(int indentSize, int indentCount)
        {
            var pad = string.Empty.PadRight(indentSize * indentCount);

            return @$"{pad}{Constants.LimitKeyword} {Size}";
        }

    }
}