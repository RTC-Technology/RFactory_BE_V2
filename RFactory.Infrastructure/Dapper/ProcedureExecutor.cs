using System.Data;
using global::Dapper;

namespace RFactory.Infrastructure.Dapper;

public class ProcedureExecutor : IProcedureExecutor
{
    private readonly IDbConnection _connection;

    public ProcedureExecutor(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<List<T>> QueryAsync<T>(string procedureName, object? parameters = null, IDbTransaction? transaction = null)
    {
        var result = await _connection.QueryAsync<T>(procedureName, parameters, transaction, commandType: CommandType.StoredProcedure);
        return result.AsList();
    }

    public async Task<T?> QuerySingleAsync<T>(string procedureName, object? parameters = null, IDbTransaction? transaction = null)
        => await _connection.QueryFirstOrDefaultAsync<T>(procedureName, parameters, transaction, commandType: CommandType.StoredProcedure);

    public async Task<int> ExecuteAsync(string procedureName, object? parameters = null, IDbTransaction? transaction = null)
        => await _connection.ExecuteAsync(procedureName, parameters, transaction, commandType: CommandType.StoredProcedure);
}
