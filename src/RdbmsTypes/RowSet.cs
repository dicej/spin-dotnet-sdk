using System.Net.WebSockets;
using SpinHttpWorld.wit.imports.fermyon.spin.v2_0_0;

namespace Spin.SDK.RdbmsTypes;

public class RowSet
{
    internal IRdbmsTypes.RowSet Value { get; }
    public List<Column> Columns { get; }
    public List<List<DbValue>> Rows { get; }

    private RowSet(IRdbmsTypes.RowSet value)
    {
        Value = value;
        Columns = value.columns.Select(Column.From).ToList();
        Rows = value.rows.Select(v => v.Select(DbValue.From).ToList()).ToList();
    }

    internal static RowSet From(IRdbmsTypes.RowSet value)
    {
        return new RowSet(value);
    }
}