using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using RFactory.Infrastructure.Dapper;
using RFactory.Infrastructure.Data;
using RFactory.Infrastructure.Data.Interceptors;
using RFactory.Infrastructure.Persistence;

namespace RFactory.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("RFactoryConnection")
            ?? throw new InvalidOperationException("Connection string 'RFactoryConnection' not found.");

        services.AddScoped<AuditableEntityInterceptor>();

        services.AddDbContext<RFactoryContext>((sp, opt) =>
        {
            opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
               .AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>());
        });

        // Dapper connection (transient, MySqlConnector opens/closes per call)
        services.AddTransient<IDbConnection>(_ => new MySqlConnection(connectionString));
        services.AddTransient<IProcedureExecutor, ProcedureExecutor>();

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

        // Scoped like the context it wraps: the transaction has to span the same DbContext
        // instance the repositories are saving through.
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
