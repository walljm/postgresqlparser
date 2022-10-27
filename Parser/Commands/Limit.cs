using System.Diagnostics.CodeAnalysis;

namespace Parser
{
    public class Limit : IClause
    {
        public static Limit All { get; } = new Limit();
        public string? Size { get; init; }

        public Limit(string size)
        {
            ArgumentNullException.ThrowIfNull(size);
            Size = size;
        }

        private Limit()
        {
        }

        public static bool TryParse(ref Tokenizer tokenizer, [NotNullWhen(true)] out Limit? limit)
        {
            limit = default;
            if (!tokenizer.TryReadKeyword(Constants.LimitKeyword))
                return false;
            (bool consumeToken, limit) = tokenizer.PeekToken() switch
            {
                NumericToken n => (true, new Limit(n.Value)),
                KeywordToken { Value: Constants.NullKeyword or Constants.AllKeyword } => (true, All),
                _ => (false, Limit.All),
            };
            if (consumeToken)
            {
                tokenizer.ReadToken();
            }
            return true;
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
            return this.Size is null ? string.Empty : @$"{pad}{Constants.LimitKeyword} {Size}";
        }
    }
}
