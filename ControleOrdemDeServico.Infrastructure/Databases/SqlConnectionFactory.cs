using Microsoft.Data.SqlClient;
using System.Data;

namespace OsService.Infrastructure.Databases;

public sealed class SqlConnectionFactory(string connectionString) : IOsServiceDbConnectionFactory,
    IMasterDbConnectionFactory
{
    public IDbConnection Create() => new SqlConnection(connectionString);
}
