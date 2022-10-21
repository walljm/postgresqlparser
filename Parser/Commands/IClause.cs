namespace Parser
{
    public interface IItem
    {
        string Print(int indentSize, int indentCount);
    }

    public interface IClause : IItem
    { }

    // types of clauses
    public interface ITable : IClause
    { }

    public interface IColumn : IItem
    {
        int FulNameLength { get; }

        string PrintWithPaddedAlias(int size);
    }
}