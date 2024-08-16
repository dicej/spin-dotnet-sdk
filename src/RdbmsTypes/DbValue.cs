using SpinHttpWorld.wit.imports.fermyon.spin.v2_0_0;

namespace Spin.SDK.RdbmsTypes;

public class DbValue
{
    private IRdbmsTypes.DbValue Value { get; }

    private DbValue(IRdbmsTypes.DbValue value)
    {
        Value = value;
    }

    public static DbValue From(IRdbmsTypes.DbValue value)
    {
        return new DbValue(value);
    }

    public byte[] AsBinary => Value.AsBinary;
    public string AsString => Value.AsStr;
    public bool AsBool => Value.AsBoolean;

    public sbyte AsSByte => Value.AsInt8;
    public short AsShort => Value.AsInt16;
    public int AsInt => Value.AsInt32;
    public long AsLong => Value.AsInt64;

    public byte AsByte => Value.AsUint8;
    public ushort AsUShort => Value.AsUint16;
    public uint AsUInt => Value.AsUint32;
    public ulong AsULong => Value.AsUint64;

    public float AsFloat => Value.AsFloating32;
    public double AsDouble => Value.AsFloating64;

    public DbDataType Type
    {
        get
        {
            return Value.Tag switch
            {
                IRdbmsTypes.DbValue.INT8 => DbDataType.SHORT_BYTE,
                IRdbmsTypes.DbValue.INT16 => DbDataType.SHORT,
                IRdbmsTypes.DbValue.INT32 => DbDataType.INT,
                IRdbmsTypes.DbValue.INT64 => DbDataType.LONG,
                IRdbmsTypes.DbValue.STR => DbDataType.STRING,
                IRdbmsTypes.DbValue.BOOLEAN => DbDataType.BOOLEAN,
                IRdbmsTypes.DbValue.UINT8 => DbDataType.BYTE,
                IRdbmsTypes.DbValue.UINT16 => DbDataType.USHORT,
                IRdbmsTypes.DbValue.UINT32 => DbDataType.UINT,
                IRdbmsTypes.DbValue.UINT64 => DbDataType.ULONG,
                IRdbmsTypes.DbValue.FLOATING32 => DbDataType.FLOAT,
                IRdbmsTypes.DbValue.FLOATING64 => DbDataType.DOUBLE,
                IRdbmsTypes.DbValue.BINARY => DbDataType.BINARY,
                _ => DbDataType.OTHER
            };
        }
    }
}