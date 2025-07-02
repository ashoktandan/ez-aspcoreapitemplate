#if mysql
using MySql.Data.MySqlClient;
using System.Data;

namespace MyTemplate.Infrastructure.Database;

public interface IMySqlConnectionFactory
{
    IDbConnection CreateConnection();
}

public class MySqlConnectionFactory : IMySqlConnectionFactory
{
    private readonly string _connectionString = "Server=localhost;Database=mydb;User=root;Password=yourpassword;Pooling=true;MinimumPoolSize=5;MaximumPoolSize=50;";

    public IDbConnection CreateConnection()
    {
        return new MySqlConnection(_connectionString);
    }
}
#endif
