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

    public interface INamedReference : IItem
    {
        string? Prefix { get; }
        string? Name { get; }
    }

    public interface IPaddedReference : INamedReference
    {
        public int FulNameLength => (Prefix?.Length ?? 0) + (Prefix == null ? 0 : 1) + (Name?.Length ?? 0);
        string PrintPadded(int size);
    }
}