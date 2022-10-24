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
                                queue.Dequeue(); // we ignore ALL, as it is the default an not necessary.
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
                        if (!queue.TryDequeue(out var bykeyword1) || bykeyword1.Value != Constants.ByKeyword)
                        {
                            throw new InvalidDataException($"{Constants.GroupKeyword} must be followed by '{Constants.ByKeyword}' keyword.");
                        }

                        this.groupBy = new GroupBy(queue);
                        continue;
                    case Constants.OrderKeyword:
                        if (!queue.TryDequeue(out var bykeyword2) || bykeyword2.Value != Constants.ByKeyword)
                        {
                            throw new InvalidDataException($"{Constants.OrderKeyword} must be followed by '{Constants.ByKeyword}' keyword.");
                        }

                        this.orderBy = new OrderBy(queue);
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
{this.columns.Print(indentSize, indentCount + 1)}
{this.from.Print(indentSize, indentCount)}{MaybePrintGroupBy(indentSize, indentCount)}{MaybePrintOrderBy(indentSize, indentCount)}
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
    }
}