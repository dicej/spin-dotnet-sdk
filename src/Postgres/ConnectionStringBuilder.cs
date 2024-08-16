namespace Spin.SDK.Postgres;

public class ConnectionStringBuilder
{
    public required string Host { get; set; }
    public required string Username { get; set; }
    public string? Password { get; set; }
    public required string Database { get; set; }
    public int Port { get; set; } = 5432;

    public string Build()
    {
        return string.IsNullOrWhiteSpace(Password)
            ? $"postgres://{Username}@{Host}:{Port}/{Database}"
            : $"postgres://{Username}:{Password}@{Host}:{Port}/{Database}";
    }
}