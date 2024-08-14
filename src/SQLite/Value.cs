using SpinHttpWorld.wit.imports.fermyon.spin.v2_0_0;

namespace Spin.SDK.SQLite;

public class Value
{

    internal static Value From(ISqlite.Value value)
    {
        return value.Tag switch
        {
            ISqlite.Value.TEXT => FromString(value.AsText),
            ISqlite.Value.BLOB => FromBlob(value.AsBlob),
            ISqlite.Value.REAL => FromDouble(value.AsReal),
            ISqlite.Value.INTEGER => FromInteger(value.AsInteger),
            _ => FromNull()
        };
    }
    
    internal ISqlite.Value Actual { get; }

    public byte Tag => Actual.Tag;
    public bool IsNull => Actual.Tag == ISqlite.Value.NULL;
    
    public long AsInteger() => Actual.AsInteger;
    public double AsDouble() => Actual.AsReal;
    public string AsText() => Actual.AsText;
    public byte[] AsBlob() => Actual.AsBlob;

    private Value(ISqlite.Value actual)
    {
        Actual = actual;
    }
    
    public static Value FromInteger(long value)
    {
        return new Value(ISqlite.Value.integer(value));
    }

    public static Value FromString(string value)
    {
        return new Value(ISqlite.Value.text(value));
    }
    
    public static Value FromDouble(double value)
    {
        return new Value(ISqlite.Value.real(value));
    }
    
    public static Value FromBlob(byte[] value)
    {
        return new Value(ISqlite.Value.blob(value));
    }
    
    public static Value FromNull()
    {
        return new Value(ISqlite.Value.@null());
    }
}