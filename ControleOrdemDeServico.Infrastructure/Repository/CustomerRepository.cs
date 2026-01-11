using OsService.Domain.Entities;
using OsService.Infrastructure.Databases;
using Dapper;

namespace OsService.Infrastructure.Repository;

public sealed class CustomerRepository(IOsServiceDbConnectionFactory factory) : ICustomerRepository
{
    public async Task InsertAsync(CustomerEntity customer, CancellationToken ct)
    {
        const string sql = @"
            INSERT INTO dbo.Customers (Id, Name, Phone, Email, Document, CreatedAt)
            VALUES (@Id, @Name, @Phone, @Email, @Document, @CreatedAt);";

        using var conn = factory.Create();
        await conn.ExecuteAsync(new CommandDefinition(sql, customer, cancellationToken: ct));
    }

    public async Task<CustomerEntity?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        const string sql = @"
            SELECT Id, Name, Phone, Email, Document, CreatedAt
            FROM dbo.Customers
            WHERE Id = @Id;";

        using var conn = factory.Create();
        return await conn.QuerySingleOrDefaultAsync<CustomerEntity>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));
    }

    public async Task<IEnumerable<CustomerEntity>> GetAllAsync(CancellationToken ct)
    {
        const string sql = @"
            SELECT Id, Name, Phone, Email, Document, CreatedAt
            FROM dbo.Customers
            ORDER BY Name ASC;";

        using var conn = factory.Create();
        return await conn.QueryAsync<CustomerEntity>(
            new CommandDefinition(sql, cancellationToken: ct));
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct)
    {
        const string sql = "SELECT 1 FROM dbo.Customers WHERE Id = @Id;";
        using var conn = factory.Create();
        var exists = await conn.QueryFirstOrDefaultAsync<int?>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));
        return exists.HasValue;
    }
}

