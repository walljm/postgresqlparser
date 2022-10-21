using System.Text;

namespace Parser
{
    public static class Extensions
    {
        public static bool CanRead(this TextReader reader)
        {
            return reader.Peek() > -1;
        }

        public static char PeekChar(this TextReader reader)
        {
            return (char)reader.Peek();
        }

        public static char ReadChar(this TextReader reader)
        {
            return (char)reader.Read();
        }

        public static string ReadWrappedString(this TextReader reader, char delimiter)
        {
            if (!reader.CanRead() && reader.PeekChar() != delimiter)
            {
                throw new InvalidOperationException("Premature end of quoted string");
            }

            var sb = new StringBuilder(); // get the opening character
            sb.Append(reader.ReadChar());
            while (reader.PeekChar() != delimiter)
            {
                if (!reader.CanRead())
                {
                    throw new InvalidOperationException("Premature end of quoted string");
                }
                sb.Append(reader.ReadChar());
            }

            sb.Append(reader.ReadChar());
            return sb.ToString();
        }

        public static int Last(this StringBuilder sb)
        {
            if (sb.Length > 0)
            {
                return sb[^1];
            }
            else return -1;
        }

        public static int FromEnd(this StringBuilder sb, int i)
        {
            if (sb.Length >= i)
            {
                return sb[^i];
            }
            else return -1;
        }
    }
}