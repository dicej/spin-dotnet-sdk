using SpinHttpWorld.wit.imports.fermyon.spin.v2_0_0;

namespace Spin.SDK.SQLite;

public class QueryResult
{
    public IList<string> Columns { get; private set; }
    public IList<RowResult> Rows { get; private set; }

    private QueryResult(IList<RowResult> rows, IList<string> columns)
    {
        Rows = rows;
        Columns = columns;
    }
    
    internal static QueryResult From(ISqlite.QueryResult result)
    {
        return new QueryResult(result.rows.Select(RowResult.From).ToList(), result.columns);
    }
}