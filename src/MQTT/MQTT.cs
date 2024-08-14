using SpinHttpWorld.wit.imports.fermyon.spin.v2_0_0;

namespace Spin.SDK.MQTT;

public class Connection: IDisposable
{
    private readonly IMqtt.Connection _connection;
    private Connection(string address, string username, string password, ulong keepAlive)
    {
        _connection = IMqtt.Connection.Open(address, username, password, keepAlive);
    }
    
    public static Connection Open(string address, string username, string password, ulong keepAliveIntervalInSecs)
    {
        return new Connection(address, username, password, keepAliveIntervalInSecs);
    }

    public void Publish(string topic, byte[] payload, Qos qos)
    {
        _connection.Publish(topic, payload, ToQos(qos));
    }

    private static IMqtt.Qos ToQos(Qos value)
    {
        return (IMqtt.Qos)(int)value;
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}

public enum Qos
{
    AT_MOST_ONCE, 
    AT_LEAST_ONCE, 
    EXACTLY_ONCE
}