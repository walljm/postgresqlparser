using System.Diagnostics.CodeAnalysis;

namespace Parser
{
    public enum TableType
    {
        TEMP,
        UNLOGGED,
    }

    public class Create : IItem
    {
        //CREATE [ [ GLOBAL | LOCAL ] { TEMPORARY | TEMP } | UNLOGGED ] TABLE [ IF NOT EXISTS ] table_name ( [
        //  { column_name data_type [ COMPRESSION compression_method ] [ COLLATE collation ] [ column_constraint [ ... ] ]
        //    | table_constraint
        //    | LIKE source_table [ like_option ... ] }
        //    [, ... ]
        //] )

        //[ INHERITS ( parent_table [, ... ] ) ]
        //[ PARTITION BY { RANGE | LIST | HASH } ( { column_name | ( expression ) } [ COLLATE collation ] [ opclass ] [, ... ] ) ]
        //[ USING method ]
        //[ WITH ( storage_parameter [= value] [, ... ] ) | WITHOUT OIDS ]
        //[ ON COMMIT { PRESERVE ROWS | DELETE ROWS | DROP } ]
        //[ TABLESPACE tablespace_name ]

        public TableType? TableType { get; init; }
        public bool HasIfNotExists { get; init; } = false;
        public string TableName {get; init;}

        public List<ColumnDefinition> ColumnDefinitions {get;init;}

        public Create()
        {
        }

        public static bool TryParse(Queue<Token> queue, [NotNullWhen(true)] out Create? create)
        {
            create = null;
            Distinct? Distinct = null;

            while (queue.TryPeek(out var token))
            {
                switch (token.Value)
                {
                    case Constants.CreateKeyword:
                        queue.Dequeue(); // drop keyword.
                        if (queue.TryPeek(out var testToken))
                        {
                            if (testToken.Value == Constants.LocalKeyword)
                            {
                                queue.Dequeue(); // we ignore this keyword, because , as it is the default an not necessary.
                            }

                            if (testToken.Value == Constants.GlobalKeyword)
                            {
                                queue.Dequeue(); // we ignore this keyword, because , as it is the default an not necessary.
                            }

                            // you need to check for distinct here.
                            if (Distinct.TryParse(queue, out var distinct))
                            {
                                Distinct = distinct;
                            }

                            if (Columns.TryParse(queue, out var columns))
                            {
                                Columns = columns;
                            }
                            else
                            {
                                throw new InvalidOperationException("Missing columns");
                            }
                        }
                        continue;
                    case Constants.HavingKeyword:

                        continue;
                    default:

                        throw new InvalidOperationException("Failed to process token! Infinite loop detected.");
                }
            }
            create = ;
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

    }
}