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
            while (queue.TryPeek(out var token))
            {
                switch (token.Value)
                {
                    case Constants.SelectKeyword:
                        queue.Dequeue(); // drop keyword.
                        if (queue.TryPeek(out var select))
                        {
                            if (select.Value == Constants.AllKeyword)
                            {
                                queue.Dequeue(); // we ignore ALL, as it is the default an not necessary.
                            }

                            // you need to check for distinct here.
                            if (Distinct.TryParse(queue, out var distinct))
                            {
                                this.distinct = distinct;
                            }

                            if (Columns.TryParse(queue, out var columns))
                            {
                                this.columns = columns ?? throw new InvalidOperationException("Null from when try parse returned true!");
                            }
                            else
                            {
                                throw new InvalidOperationException("Missing columns");
                            }
                        }
                        continue;
                    case Constants.FromKeyword:
                        if (From.TryParse(queue, out var from))
                        {
                            this.from = from ?? throw new InvalidOperationException("Null from when try parse returned true!");
                        }
                        continue;
                    case Constants.WhereKeyword:
                        if (Where.TryParse(queue, out var where))
                        {
                            this.where = where;
                        }
                        continue;
                    case Constants.GroupKeyword:
                        if (GroupBy.TryParse(queue, out var groupBy))
                        {
                            this.groupBy = groupBy;
                        }
                        continue;
                    case Constants.OrderKeyword:
                        if (OrderBy.TryParse(queue, out var orderBy))
                        {
                            this.orderBy = orderBy;
                        }
                        continue;
                    case Constants.OffsetKeyword:
                        if (Offset.TryParse(queue, out var offset))
                        {
                            this.offset = offset;
                        }
                        continue;
                    case Constants.LimitKeyword:
                        if (Limit.TryParse(queue, out var limit))
                        {
                            this.limit = limit;
                        }
                        continue;
                    case Constants.HavingKeyword:

                        continue;
                    default:
                        throw new InvalidOperationException("Unknown keyword!");
                }
            }
        }

        public string Print(int indentSize, int indentCount)
        {
            var indent = indentSize * indentCount;
            var pad = string.Empty.PadLeft(indent);
            return $@"{pad}SELECT{this.distinct?.Print(indentSize, indentCount) ?? string.Empty}
{this.columns.Print(indentSize, indentCount + 1)}
{this.from.Print(indentSize, indentCount)}{MaybePrintWhere(indentSize, indentCount)}{MaybePrintGroupBy(indentSize, indentCount)}{MaybePrintOrderBy(indentSize, indentCount)}{MaybePrintOffset(indentSize, indentCount)}{MaybePrintLimit(indentSize, indentCount)}
";
        }

        private string MaybePrintGroupBy(int indentSize, int indentCount)
        {
            return $"{(this.groupBy != null ? Environment.NewLine : string.Empty)}{this.groupBy?.Print(indentSize, indentCount)}";
        }

        private string MaybePrintOrderBy(int indentSize, int indentCount)
        {
            return $"{(this.orderBy != null ? Environment.NewLine : string.Empty)}{this.orderBy?.Print(indentSize, indentCount)}";
        }

        private string MaybePrintLimit(int indentSize, int indentCount)
        {
            return $"{(this.limit != null ? Environment.NewLine : string.Empty)}{this.limit?.Print(indentSize, indentCount)}";
        }
        
        private string MaybePrintOffset(int indentSize, int indentCount)
        {
            return $"{(this.limit != null ? Environment.NewLine : string.Empty)}{this.offset?.Print(indentSize, indentCount)}";
        }
        private string MaybePrintWhere(int indentSize, int indentCount)
        {
            return $"{(this.where != null ? Environment.NewLine : string.Empty)}{this.where?.Print(indentSize, indentCount)}";
        }
    }
}