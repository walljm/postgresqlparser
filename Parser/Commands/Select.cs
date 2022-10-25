namespace Parser
{
    public class Select : IItem
    {
        // SELECT [ ALL | DISTINCT [ ON ( expression [, ...] ) ] ]
        // [ * | expression [ [ AS ] output_name ] [, ...] ]
        // [ FROM from_item [, ...] ]
        // [ WHERE condition ]
        // [ GROUP BY [ ALL | DISTINCT ] grouping_element [, ...] ]
        // [ ORDER BY expression [ ASC | DESC | USING operator ] [ NULLS { FIRST | LAST } ] [, ...] ]
        // [ LIMIT { count | ALL } ]
        // [ OFFSET start [ ROW | ROWS ] ]

        public Distinct? Distinct { get; init; }
        public Columns Columns { get; init; }
        public From From { get; init; }
        public Where? Where { get; init; }
        public GroupBy? GroupBy { get; init; }
        public OrderBy? OrderBy { get; init; }
        public Limit? Limit { get; init; }
        public Offset? Offset { get; init; }

        // [ HAVING condition ]
        public IClause? having;

        public Select(Columns cols, From from)
        {
            Columns = cols;
            From = from;
        }

        public static bool TryParse(Queue<Token> queue, out Select? select)
        {
            select = null;
            Distinct? Distinct = null;
            Columns? Columns = null;
            From? From = null;
            Where? Where = null;
            GroupBy? GroupBy = null;
            OrderBy? OrderBy = null;
            Limit? Limit = null;
            Offset? Offset = null;

            while (queue.TryPeek(out var token))
            {
                switch (token.Value)
                {
                    case Constants.SelectKeyword:
                        queue.Dequeue(); // drop keyword.
                        if (queue.TryPeek(out var selectToken))
                        {
                            if (selectToken.Value == Constants.AllKeyword)
                            {
                                queue.Dequeue(); // we ignore ALL, as it is the default an not necessary.
                            }

                            // you need to check for distinct here.
                            if (Distinct.TryParse(queue, out var distinct))
                            {
                                Distinct = distinct;
                            }

                            if (Columns.TryParse(queue, out var columns))
                            {
                                Columns = columns ?? throw new InvalidOperationException("Null from when try parse returned true!");
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
                            From = from ?? throw new InvalidOperationException("Null from when try parse returned true!");
                        }
                        continue;
                    case Constants.WhereKeyword:
                        if (Where.TryParse(queue, out var where))
                        {
                            Where = where;
                        }
                        continue;
                    case Constants.GroupKeyword:
                        if (GroupBy.TryParse(queue, out var groupBy))
                        {
                            GroupBy = groupBy;
                        }
                        continue;
                    case Constants.OrderKeyword:
                        if (OrderBy.TryParse(queue, out var orderBy))
                        {
                            OrderBy = orderBy;
                        }
                        continue;
                    case Constants.OffsetKeyword:
                        if (Offset.TryParse(queue, out var offset))
                        {
                            Offset = offset;
                        }
                        continue;
                    case Constants.LimitKeyword:
                        if (Limit.TryParse(queue, out var limit))
                        {
                            Limit = limit;
                        }
                        continue;
                    case Constants.HavingKeyword:

                        continue;
                    default:

                        throw new InvalidOperationException("Failed to process token! Infinite loop detected.");
                }
            }
            if (Columns == null)
            {
                throw new InvalidOperationException("You must have columns in a Select statement!");
            }
            if (From == null)
            {
                throw new InvalidOperationException("You must have a FROM clause in a SELECT statement!");
            }

            select = new Select(
                Columns,
                From
                )
            {
                Distinct = Distinct,
                Where = Where,
                GroupBy = GroupBy,
                OrderBy = OrderBy,
                Limit = Limit,
                Offset = Offset,
            };
            return true;
        }

        public string Print(int indentSize, int indentCount)
        {
            var indent = indentSize * indentCount;
            var pad = string.Empty.PadLeft(indent);
            return $@"{pad}SELECT{this.Distinct?.Print(indentSize, indentCount) ?? string.Empty}
{this.Columns.Print(indentSize, indentCount + 1)}
{this.From.Print(indentSize, indentCount)}{MaybePrintWhere(indentSize, indentCount)}{MaybePrintGroupBy(indentSize, indentCount)}{MaybePrintOrderBy(indentSize, indentCount)}{MaybePrintOffset(indentSize, indentCount)}{MaybePrintLimit(indentSize, indentCount)}
";
        }

        private string MaybePrintGroupBy(int indentSize, int indentCount)
        {
            return $"{(this.GroupBy != null ? Environment.NewLine : string.Empty)}{this.GroupBy?.Print(indentSize, indentCount)}";
        }

        private string MaybePrintOrderBy(int indentSize, int indentCount)
        {
            return $"{(this.OrderBy != null ? Environment.NewLine : string.Empty)}{this.OrderBy?.Print(indentSize, indentCount)}";
        }

        private string MaybePrintLimit(int indentSize, int indentCount)
        {
            return $"{(this.Limit != null ? Environment.NewLine : string.Empty)}{this.Limit?.Print(indentSize, indentCount)}";
        }

        private string MaybePrintOffset(int indentSize, int indentCount)
        {
            return $"{(this.Limit != null ? Environment.NewLine : string.Empty)}{this.Offset?.Print(indentSize, indentCount)}";
        }

        private string MaybePrintWhere(int indentSize, int indentCount)
        {
            return $"{(this.Where != null ? Environment.NewLine : string.Empty)}{this.Where?.Print(indentSize, indentCount)}";
        }
    }
}