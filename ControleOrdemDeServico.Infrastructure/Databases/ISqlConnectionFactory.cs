using System.Data;

namespace OsService.Infrastructure.Databases;

public interface IOsServiceDbConnectionFactory
{
    IDbConnection Create();
}

public interface IMasterDbConnectionFactory
{
    IDbConnection Create();
}
