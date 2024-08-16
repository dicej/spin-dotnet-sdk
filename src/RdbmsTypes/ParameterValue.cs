using SpinHttpWorld.wit.imports.fermyon.spin.v2_0_0;

namespace Spin.SDK.RdbmsTypes;

public class ParameterValue
{
    private ParameterValue(IRdbmsTypes.ParameterValue inner)
    {
        Value = inner;
    }
    
    public static ParameterValue FromShortByte(sbyte value)
    {
        return new ParameterValue(IRdbmsTypes.ParameterValue.int8(value));
    }
    
    public static ParameterValue FromShort(short value)
    {
        return new ParameterValue(IRdbmsTypes.ParameterValue.int16(value));
    }
    
    public static ParameterValue FromInt(int value)
    {
        return new ParameterValue(IRdbmsTypes.ParameterValue.int32(value));
    }
    
    public static ParameterValue FromLong(long value)
    {
        return new ParameterValue(IRdbmsTypes.ParameterValue.int64(value));
    }
    
    public static ParameterValue FromByte(byte value)
    {
        return new ParameterValue(IRdbmsTypes.ParameterValue.uint8(value));
    }
    
    public static ParameterValue FromUShort(ushort value)
    {
        return new ParameterValue(IRdbmsTypes.ParameterValue.uint16(value));
    }
    
    public static ParameterValue FromUInt(uint value)
    {
        return new ParameterValue(IRdbmsTypes.ParameterValue.uint32(value));
    }
    
    public static ParameterValue FromULong(ulong value)
    {
        return new ParameterValue(IRdbmsTypes.ParameterValue.uint64(value));
    }
    
    public static ParameterValue FromBinary(byte[] value)
    {
        return new ParameterValue(IRdbmsTypes.ParameterValue.binary(value));
    }
    
    public static ParameterValue FromBool(bool value)
    {
        return new ParameterValue(IRdbmsTypes.ParameterValue.boolean(value));
    }
    
    public static ParameterValue FromFloat(float value)
    {
        return new ParameterValue(IRdbmsTypes.ParameterValue.floating32(value));
    }
    
    public static ParameterValue FromDouble(double value)
    {
        return new ParameterValue(IRdbmsTypes.ParameterValue.floating64(value));
    }
    
    public static ParameterValue FromString(string value)
    {
        return new ParameterValue(IRdbmsTypes.ParameterValue.str(value));
    }
    
    public static ParameterValue DbNull()
    {
        return new ParameterValue(IRdbmsTypes.ParameterValue.dbNull());
    }

    internal IRdbmsTypes.ParameterValue Value { get; }
}