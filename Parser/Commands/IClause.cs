namespace Parser
{
    public interface IItem
    {
        string Print(int indent);
    }

    public interface IClause : IItem
    { }

    // types of clauses
    public interface ITable : IClause
    { }
}