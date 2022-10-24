namespace Parser
{
    public class Offset : IClause
    {
        public string? Size { get; init; }

        public Offset(string? size)
        {
            this.Size = size;
        }

        public static bool TryParse(Queue<Token> queue, out Offset? offset)
        {
            offset = null;
            if (queue.TryPeek(out var token) && token.Value == Constants.OffsetKeyword)
            {
                queue.Dequeue();
                if (queue.TryPeek(out var size) && size is NumericToken)
                {
                    queue.Dequeue();
                    offset = new Offset(size.Value);
                    return true;
                }
                else
                {
                    throw new ArgumentException($"Invalid {Constants.OffsetKeyword} value.");
                }
            }
            return false;
        }

        public string Print(int indentSize, int indentCount)
        {
            var pad = string.Empty.PadRight(indentSize * indentCount);

            return @$"{pad}{Constants.OffsetKeyword} {Size}";
        }
    }
}