#if postgres
using Npgsql;
using System.Data;

namespace MyTemplate.Infrastructure.Database;

public interface IPostgresConnectionFactory
{
    IDbConnection CreateConnection();
}

public class PostgresConnectionFactory : IPostgresConnectionFactory
{
    private readonly string _connectionString = "Host=localhost;Database=mydb;Username=postgres;Password=yourpassword;Pooling=true;Minimum Pool Size=5;Maximum Pool Size=50;";

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
#endif
