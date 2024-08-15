using SpinHttpWorld.wit.imports.fermyon.spin.v2_0_0;

namespace Fermyon.Spin.SQLite;

public class RowResult
{
    public readonly List<Value> Values;

    private RowResult(List<Value> values)
    {
        Values = values;
    }
    internal static RowResult From(ISqlite.RowResult rowResult)
    {
        return new RowResult(rowResult.values.Select(Value.From).ToList());
    }
}
