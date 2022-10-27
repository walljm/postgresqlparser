using System.Diagnostics.CodeAnalysis;

namespace Parser
{
    public class Offset : IClause
    {
        public static Offset Zero { get; } = new Offset();
        public string? Size { get; }

        public Offset(string size)
        {
            ArgumentNullException.ThrowIfNull(size);
            this.Size = size;
        }

        private Offset()
        {
        }

        public static bool TryParse(ref Tokenizer tokenizer, [NotNullWhen(true)] out Offset? offset)
        {
            offset = default;
            if (!tokenizer.TryReadKeyword(Constants.OffsetKeyword))
                return false;
            (bool consumeToken, offset) = tokenizer.PeekToken() switch
            {
                NumericToken n => (true, new Offset(n.Value)),
                KeywordToken { Value: Constants.NullKeyword } => (true, Zero),
                _ => (false, Zero),
            };
            if (consumeToken)
            {
                tokenizer.ReadToken();
            }
            return true;
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
            return Size is null ? string.Empty : @$"{pad}{Constants.OffsetKeyword} {Size}";
        }
    }
}
