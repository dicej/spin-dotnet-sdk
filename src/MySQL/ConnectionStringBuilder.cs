namespace Spin.SDK.MySQL;

public class ConnectionStringBuilder
{
    public required string Hostname { get; set; }
    public required string Username { get; set; }
    public string? Password { get; set; }
    public required string DatabaseName { get; set; }
    public int Port { get; set; } = 3306;

    public string Build()
    {
        return string.IsNullOrWhiteSpace(Password)
            ? $"mysql://{Username}@{Hostname}:{Port}/{DatabaseName}"
            : $"mysql://{Username}:{Password}@{Hostname}:{Port}/{DatabaseName}";
    }
}