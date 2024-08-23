using System;
using System.Collections.Generic;
using System.Linq;
using SpinHttpWorld.wit.imports.fermyon.spin.v2_0_0;

namespace Spin.SDK.SQLite;

public class Connection: IDisposable
{
    private const string DEFAULT_DATABASE_NAME = "default";
    private readonly ISqlite.Connection _connection;
    
    private Connection(ISqlite.Connection connection)
    {
        _connection = connection;
    }
    
    public static Connection Open(string databaseName)
    {
        return new Connection(ISqlite.Connection.Open(databaseName));
    }

    public static Connection OpenDefault()
    {
        return Open(DEFAULT_DATABASE_NAME);
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    public QueryResult Execute(string statement, IEnumerable<Value> parameters)
    {
        var result = _connection.Execute(statement, parameters.Select(p => p.Actual).ToList());
        return QueryResult.From(result);
    }
}
