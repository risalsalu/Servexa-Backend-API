using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Servexa.Application.Interfaces;

namespace Servexa.Infrastructure.Services;

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
                           ?? throw new InvalidOperationException("Connection string not found.");
    }

    public IDbConnection CreateConnection()
        => new SqlConnection(_connectionString);
}
