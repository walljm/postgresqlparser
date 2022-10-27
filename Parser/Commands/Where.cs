namespace Parser
{
    public class Where : IClause
    {
        public List<(string? Join, Conditional Condition)> Conditions { get; init; }

        public Where(List<(string? Join, Conditional Condition)> conditions)
        {
            this.Conditions = conditions;
        }

        public static bool TryParse(Queue<Token> queue, out Where? where)
        {
            where = null;

            if (queue.TryPeek(out var test) && test is KeywordToken && test.Value == Constants.WhereKeyword)
            {
                queue.Dequeue(); //
            }
            else
            {
                return false;
            }

            var conditions = new List<(string? Join, Conditional Condition)>();
            string? join = null;
            while (queue.TryPeek(out var token) && token is IdentifierToken)
            {
                if (!Conditional.TryParse(queue, out var conditional))
                {
                    throw new InvalidOperationException("Expected a column reference");
                }

                if (queue.TryPeek(out var nextColumn) && nextColumn.Value is Constants.AndKeyword or Constants.OrKeyword)
                {
                    conditions.Add((join, conditional));
                    join = nextColumn.Value;
                    queue.Dequeue(); // remove the next item indicator so you're ready to check the next thing.
                    continue; // you have more columns to process
                }

                conditions.Add((join, conditional));
                where = new Where(conditions);
                return true; // exit the loop
            }

            where = null;
            return false;
        }

        public string Print(int indentSize, int indentCount)
        {
            var pad = string.Empty.PadRight(indentSize * indentCount);
            var innerPad = string.Empty.PadRight(indentSize * (indentCount + 1));
            return @$"{pad}{Constants.WhereKeyword}
{innerPad}{string.Join(Environment.NewLine + innerPad, Conditions.Select(o => (o.Join == null ? string.Empty : o.Join + " ") + o.Condition.Print(0, 0)))}";
        }
    }
}
