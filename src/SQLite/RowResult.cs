using System.Collections.Generic;
using System.Linq;
using SpinHttpWorld.wit.imports.fermyon.spin.v2_0_0;

namespace Spin.SDK.SQLite;

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
