using System;
using System.Collections.Generic;
using System.Linq;
using Spin.SDK.RdbmsTypes;
using SpinHttpWorld.wit.imports.fermyon.spin.v2_0_0;

namespace Spin.SDK.MySQL;

public class Connection: IDisposable
{
    private readonly IMysql.Connection _connection;

    private Connection(IMysql.Connection connection)
    {
        _connection = connection;
    }

    public static Connection Open(string connectionString)
    {
        return new Connection(IMysql.Connection.Open(connectionString));
    }

    public void Execute(string statement, List<ParameterValue> parameters)
    {
        _connection.Execute(statement, parameters.Select(p => p.Value).ToList());
    }

    public RowSet Query(string query, List<ParameterValue> parameters)
    {
        var value = _connection.Query(query, parameters.Select(p => p.Value).ToList());

        return RowSet.From(value);
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
 
