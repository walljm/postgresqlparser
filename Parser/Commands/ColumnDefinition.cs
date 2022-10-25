namespace Parser
{
    public class ColumnDefinition : IItem
    {
        // { column_name data_type [ COMPRESSION compression_method ] [ COLLATE collation ] [ column_constraint [ ... ] ]
        // where column_constraint is:
        //[ CONSTRAINT constraint_name ]
        //{ NOT NULL |
        //  NULL |
        //  CHECK ( expression ) [ NO INHERIT ] |
        //  DEFAULT default_expr |
        //  GENERATED ALWAYS AS ( generation_expr ) STORED |
        //  GENERATED { ALWAYS | BY DEFAULT } AS IDENTITY [ ( sequence_options ) ] |
        //  UNIQUE [ NULLS [ NOT ] DISTINCT ] index_parameters |
        //  PRIMARY KEY index_parameters |
        //  REFERENCES reftable [ ( refcolumn ) ] [ MATCH FULL | MATCH PARTIAL | MATCH SIMPLE ]
        //    [ ON DELETE referential_action ] [ ON UPDATE referential_action ] }
        //[ DEFERRABLE | NOT DEFERRABLE ] [ INITIALLY DEFERRED | INITIALLY IMMEDIATE ]

        public string ColumnName { get; init; }
        public string DataType { get; init; }

        public ColumnConstraint? Constraint { get; init; }

        public ColumnDefinition()
        {
        }

        public static bool TryParse(Queue<Token> queue, out ColumnDefinition? columns)
        {
            columns = null;
            return false;
        }

        public string Print(int indentSize, int indentCount)
        {
            var pad = string.Empty.PadRight(indentSize * indentCount);

            return @$"{pad}";
        }
    }

    public class ColumnConstraint : IItem
    {
        //[ CONSTRAINT constraint_name ]
        public string Name { get; init; }

        //{ NOT NULL |
        //  NULL |
        public bool AllowNull { get; init; }

        //  CHECK ( expression ) [ NO INHERIT ] |
        public IItem? Check { get; init; }

        //  DEFAULT default_expr |
        public IItem? Default { get; init; }

        //  GENERATED ALWAYS AS ( generation_expr ) STORED |
        public IItem? GeneratedAlwaysAsStored { get; init; }

        //  GENERATED { ALWAYS | BY DEFAULT } AS IDENTITY [ ( sequence_options ) ] |
        public IItem? GeneratedAsIdentity { get; init; }

        //  UNIQUE [ NULLS [ NOT ] DISTINCT ] index_parameters |
        public IItem? Unique { get; init; }

        //  PRIMARY KEY index_parameters |
        public IItem? PrimaryKey { get; init; }

        //  REFERENCES reftable [ ( refcolumn ) ] [ MATCH FULL | MATCH PARTIAL | MATCH SIMPLE ]
        public IItem? References { get; init; }

        //    [ ON DELETE referential_action ] [ ON UPDATE referential_action ] }
        public IItem? OnDelete { get; init; }

        public IItem? OnUpdate { get; init; }

        //[ DEFERRABLE | NOT DEFERRABLE ] [ INITIALLY DEFERRED | INITIALLY IMMEDIATE ]
        public bool? IsDeferable { get; init; }

        public bool? InitiallyDeferred { get; init; }
        public bool? InitiallyImmediate { get; init; }

        public ColumnConstraint()
        {
        }

        public static bool TryParse(Queue<Token> queue, out ColumnDefinition? columns)
        {
            columns = null;
            return false;
        }

        public string Print(int indentSize, int indentCount)
        {
            var pad = string.Empty.PadRight(indentSize * indentCount);

            return @$"{pad}";
        }
    }
}