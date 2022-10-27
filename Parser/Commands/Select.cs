using System.Diagnostics.CodeAnalysis;

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
        public Limit Limit { get; init; } = Limit.All;
        public Offset Offset { get; init; } = Offset.Zero;

        public Select(Columns cols, From from)
        {
            Columns = cols;
            From = from;
        }

        public static bool TryParse(ref Tokenizer tokenizer, [NotNullWhen(true)] out Select? select)
        {
            var backtrack = tokenizer;
            select = default;
            Distinct? distinct = null;
            From? from = null;
            Where? where = null;
            GroupBy? groupBy = null;
            OrderBy? orderBy = null;
            Limit? limit = null;
            Offset? offset = null;
            if (!tokenizer.TryReadKeyword(Constants.SelectKeyword))
                return false;
            if (
                tokenizer.TryPeekToken(out var token)
                && token is KeywordToken { Value: var kwVal }
            )
            {
                switch (kwVal)
                {
                    case Constants.AllKeyword:
                        tokenizer.ReadToken();
                        break;
                    case Constants.DistinctKeyword when Distinct.TryParse(ref tokenizer, out distinct):
                        break;
                    case Constants.DistinctKeyword:
                        return tokenizer.BacktrackTo(backtrack);
                }
            }

            if (!Columns.TryParse(ref tokenizer, out var columns))
            {
                return tokenizer.BacktrackTo(backtrack);
            }

            while (tokenizer.TryPeekToken(out token) && token is KeywordToken { Value: var keywordValue })
            {
                var isValid = keywordValue switch
                {
                    Constants.FromKeyword when from is not null => false,
                    Constants.FromKeyword => From.TryParse(ref tokenizer, out from),
                    Constants.WhereKeyword when where is not null => false,
                    Constants.WhereKeyword => Where.TryParse(ref tokenizer, out where),
                    Constants.GroupKeyword when groupBy is not null => false,
                    Constants.GroupKeyword => GroupBy.TryParse(ref tokenizer, out groupBy),
                    Constants.OrderKeyword when orderBy is not null => false,
                    Constants.OrderKeyword => OrderBy.TryParse(ref tokenizer, out orderBy),
                    Constants.OffsetKeyword when offset is not null => false,
                    Constants.OffsetKeyword => Offset.TryParse(ref tokenizer, out offset),
                    Constants.LimitKeyword when limit is not null => false,
                    Constants.LimitKeyword => Limit.TryParse(ref tokenizer, out limit),
                    Constants.HavingKeyword => throw new NotImplementedException(),
                    _ => false, // Unknown keyword was provided
                };
                if (isValid is false)
                {
                    return tokenizer.BacktrackTo(backtrack);
                }
            }
            if (from is null)
            {
                // columns and from are required
                return tokenizer.BacktrackTo(backtrack);
            }

            select = new Select(
                columns,
                from
            )
            {
                Distinct = distinct,
                Where = where,
                GroupBy = groupBy,
                OrderBy = orderBy,
                Limit = limit ?? Limit.All,
                Offset = offset ?? Offset.Zero,
            };
            return true;

        }
        public static bool TryParse(Queue<Token> queue, [NotNullWhen(true)] out Select? select)
        {
            select = null;
            Distinct? distinct = null;
            Columns? columns = null;
            From? from = null;
            Where? where = null;
            GroupBy? groupBy = null;
            OrderBy? orderBy = null;
            Limit? limit = null;
            Offset? offset = null;

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
                            _ = Distinct.TryParse(queue, out distinct);

                            if (!Columns.TryParse(queue, out columns))
                            {
                                throw new InvalidOperationException("Missing columns");
                            }
                        }
                        continue;
                    case Constants.FromKeyword:
                        _ = From.TryParse(queue, out from);
                        continue;
                    case Constants.WhereKeyword:
                        _ = Where.TryParse(queue, out where);
                        continue;
                    case Constants.GroupKeyword:
                        _ = GroupBy.TryParse(queue, out groupBy);
                        continue;
                    case Constants.OrderKeyword:
                        _ = OrderBy.TryParse(queue, out orderBy);
                        continue;
                    case Constants.OffsetKeyword:
                        _ = Offset.TryParse(queue, out offset);
                        continue;
                    case Constants.LimitKeyword:
                        _ = Limit.TryParse(queue, out limit);
                        continue;
                    case Constants.HavingKeyword:

                        continue;
                    default:

                        throw new InvalidOperationException("Failed to process token! Infinite loop detected.");
                }
            }
            if (columns == null)
            {
                throw new InvalidOperationException("You must have columns in a Select statement!");
            }
            if (from == null)
            {
                throw new InvalidOperationException("You must have a FROM clause in a SELECT statement!");
            }

            select = new Select(
                columns,
                from
                )
            {
                Distinct = distinct,
                Where = where,
                GroupBy = groupBy,
                OrderBy = orderBy,
                Limit = limit ?? Limit.All,
                Offset = offset ?? Offset.Zero,
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
            return $"{(this.Limit != Limit.All ? Environment.NewLine : string.Empty)}{this.Limit.Print(indentSize, indentCount)}";
        }

        private string MaybePrintOffset(int indentSize, int indentCount)
        {
            return $"{(this.Offset != Offset.Zero ? Environment.NewLine : string.Empty)}{this.Offset.Print(indentSize, indentCount)}";
        }

        private string MaybePrintWhere(int indentSize, int indentCount)
        {
            return $"{(this.Where != null ? Environment.NewLine : string.Empty)}{this.Where?.Print(indentSize, indentCount)}";
        }

    }
}
