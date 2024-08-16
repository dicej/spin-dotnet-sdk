using SpinHttpWorld.wit.imports.fermyon.spin.v2_0_0;

namespace Spin.SDK.RdbmsTypes;

public class Column
{
    private IRdbmsTypes.Column Value { get; }

    private Column(IRdbmsTypes.Column value)
    {
        Value = value;
    }
    public static Column From(IRdbmsTypes.Column value)
    {
        return new Column(value);
    }

    public string Name => Value.name;
    public DbDataType DataType => (DbDataType)(int)Value.dataType;
}