using System;
using System.Collections.Generic;
using System.Linq;
using Spin.SDK.RdbmsTypes;
using SpinHttpWorld.wit.imports.fermyon.spin.v2_0_0;

namespace Spin.SDK.Postgres;

public class Connection : IDisposable
{
    private readonly IPostgres.Connection _connection;

    private Connection(IPostgres.Connection connection)
    {
        _connection = connection;
    }

    public static Connection Open(string connectionString)
    {
        return new Connection(IPostgres.Connection.Open(connectionString));
    }

    public ulong Execute(string statement, List<ParameterValue> parameters)
    {
        return _connection.Execute(statement, parameters.Select(p => p.Value).ToList());
    }

    public RowSet Query(string query, List<ParameterValue> parameters)
    {
        var rowSet = _connection.Query(query, parameters.Select(p => p.Value).ToList());
        return RowSet.From(rowSet);
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
