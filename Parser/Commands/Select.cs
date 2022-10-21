namespace Parser
{
    public class Select : IItem
    {
        // SELECT [ ALL | DISTINCT [ ON ( expression [, ...] ) ] ]
        private readonly IClause? distinct;

        // [ * | expression [ [ AS ] output_name ] [, ...] ]
        private readonly IClause columns;

        // [ FROM from_item [, ...] ]
        private readonly IClause from;

        // [ WHERE condition ]
        private readonly IClause? where;

        // [ HAVING condition ]
        private readonly IClause? having;

        // [ GROUP BY [ ALL | DISTINCT ] grouping_element [, ...] ]
        private readonly IClause? groupBy;

        // [ ORDER BY expression [ ASC | DESC | USING operator ] [ NULLS { FIRST | LAST } ] [, ...] ]
        private readonly IClause? orderBy;

        // [ LIMIT { count | ALL } ]
        private readonly IClause? limit;

        // [ OFFSET start [ ROW | ROWS ] ]
        private readonly IClause? offset;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public Select(Queue<Token> queue)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            while (queue.TryDequeue(out var token))
            {
                switch (token.Value)
                {
                    case Constants.SelectKeyword:
                        if (queue.TryPeek(out var select))
                        {
                            if (select.Value == Constants.AllKeyword)
                            {
                                queue.Dequeue(); // we ignore ALL
                            }

                            if (select.Value == Constants.DistinctKeyword)
                            {
                                this.distinct = new Distinct(queue);
                            }

                            // you need to check for distinct here.
                            this.columns = new Columns(queue);
                        }
                        continue;
                    case Constants.FromKeyword:
                        this.from = new From(queue);
                        continue;
                    case Constants.WhereKeyword:
                        continue;
                    case Constants.GroupKeyword:
                        continue;
                    case Constants.OrderKeyword:
                        continue;
                    case Constants.LimitKeyword:
                        continue;
                    case Constants.HavingKeyword:
                        continue;
                    case Constants.OffsetKeyword:
                        continue;
                }
            }
        }

        public string Print(int indentSize, int indentCount)
        {
            var indent = indentSize * indentCount;
            var pad = string.Empty.PadLeft(indent);
            return $@"{pad}SELECT{this.distinct?.Print(indentSize, indentCount) ?? string.Empty}
{this.columns.Print(indentSize, indentCount  + 1)}
{this.from.Print(indentSize, indentCount)}
";
        }
    }
}