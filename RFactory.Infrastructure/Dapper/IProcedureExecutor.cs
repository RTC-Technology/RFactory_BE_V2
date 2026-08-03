using System.Data;

namespace RFactory.Infrastructure.Dapper;

/// <summary>
/// Thin Dapper surface for stored procedures and raw SQL, used by reporting/dashboard reads
/// that bypass EF Core.
/// </summary>
public interface IProcedureExecutor
{
    Task<List<T>> QueryAsync<T>(string procedureName, object? parameters = null, IDbTransaction? transaction = null);
    Task<T?> QuerySingleAsync<T>(string procedureName, object? parameters = null, IDbTransaction? transaction = null);
    Task<int> ExecuteAsync(string procedureName, object? parameters = null, IDbTransaction? transaction = null);
}
