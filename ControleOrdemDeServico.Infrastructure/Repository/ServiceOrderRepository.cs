using OsService.Domain.Entities;
using OsService.Domain.Enums;
using OsService.Infrastructure.Databases;
using Dapper;

namespace OsService.Infrastructure.Repository;

public sealed class ServiceOrderRepository(IOsServiceDbConnectionFactory factory) : IServiceOrderRepository
{
    public async Task<(Guid Id, int Number)> InsertAsync(ServiceOrderEntity serviceOrder, CancellationToken ct)
    {
        const string sql = @"
INSERT INTO dbo.ServiceOrders (Id, CustomerId, Description, Status, OpenedAt, StartedAt, FinishedAt, Price, Coin, UpdatedPriceAt)
OUTPUT INSERTED.Id, INSERTED.Number
VALUES (@Id, @CustomerId, @Description, @Status, @OpenedAt, @StartedAt, @FinishedAt, @Price, @Coin, @UpdatedPriceAt);";

        using var conn = factory.Create();
        var row = await conn.QuerySingleAsync<(Guid Id, int Number)>(
            new CommandDefinition(sql, new
            {
                serviceOrder.Id,
                serviceOrder.CustomerId,
                serviceOrder.Description,
                Status = (int)serviceOrder.Status,
                serviceOrder.OpenedAt,
                serviceOrder.StartedAt,
                serviceOrder.FinishedAt,
                serviceOrder.Price,
                serviceOrder.Coin,
                serviceOrder.UpdatedPriceAt
            }, cancellationToken: ct));

        return (row.Id, row.Number);
    }

    public async Task<ServiceOrderEntity?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        const string sql = @"
SELECT 
    so.Id, 
    so.Number, 
    so.CustomerId, 
    c.Name as CustomerName,
    so.Description, 
    so.Status, 
    so.OpenedAt, 
    so.StartedAt, 
    so.FinishedAt, 
    so.Price, 
    so.Coin, 
    so.UpdatedPriceAt
FROM dbo.ServiceOrders so
INNER JOIN dbo.Customers c ON so.CustomerId = c.Id
WHERE so.Id = @Id;";

        using var conn = factory.Create();
        var raw = await conn.QuerySingleOrDefaultAsync<dynamic>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));

        if (raw is null) return null;

        return new ServiceOrderEntity
        {
            Id = raw.Id,
            Number = raw.Number,
            CustomerId = raw.CustomerId,
            CustomerName = raw.CustomerName,
            Description = raw.Description,
            Status = (ServiceOrderStatus)(int)raw.Status,
            OpenedAt = raw.OpenedAt,
            StartedAt = raw.StartedAt,
            FinishedAt = raw.FinishedAt,
            Price = raw.Price,
            Coin = raw.Coin ?? "BRL",
            UpdatedPriceAt = raw.UpdatedPriceAt
        };
    }

    public async Task<IEnumerable<ServiceOrderEntity>> GetAllAsync(CancellationToken ct)
    {
        const string sql = @"
SELECT 
    so.Id, 
    so.Number, 
    so.CustomerId, 
    c.Name as CustomerName,
    so.Description, 
    so.Status, 
    so.OpenedAt, 
    so.StartedAt, 
    so.FinishedAt, 
    so.Price, 
    so.Coin, 
    so.UpdatedPriceAt
FROM dbo.ServiceOrders so
INNER JOIN dbo.Customers c ON so.CustomerId = c.Id
ORDER BY so.Number DESC;";

        using var conn = factory.Create();
        var results = await conn.QueryAsync<dynamic>(
            new CommandDefinition(sql, cancellationToken: ct));

        return results.Select(raw => new ServiceOrderEntity
        {
            Id = raw.Id,
            Number = raw.Number,
            CustomerId = raw.CustomerId,
            CustomerName = raw.CustomerName,
            Description = raw.Description,
            Status = (ServiceOrderStatus)(int)raw.Status,
            OpenedAt = raw.OpenedAt,
            StartedAt = raw.StartedAt,
            FinishedAt = raw.FinishedAt,
            Price = raw.Price,
            Coin = raw.Coin ?? "BRL",
            UpdatedPriceAt = raw.UpdatedPriceAt
        });
    }

    public async Task<bool> UpdateStatusAsync(Guid id, int status, DateTime? startedAt, DateTime? finishedAt, CancellationToken ct)
    {
        const string sql = @"
UPDATE dbo.ServiceOrders
SET Status = @Status, StartedAt = @StartedAt, FinishedAt = @FinishedAt
WHERE Id = @Id;";

        using var conn = factory.Create();
        var rows = await conn.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id, Status = status, StartedAt = startedAt, FinishedAt = finishedAt }, cancellationToken: ct));

        return rows > 0;
    }

    public async Task<bool> UpdatePriceAsync(Guid id, decimal price, DateTime updatedAt, CancellationToken ct)
    {
        const string sql = @"
UPDATE dbo.ServiceOrders
SET Price = @Price, UpdatedPriceAt = @UpdatedAt
WHERE Id = @Id;";

        using var conn = factory.Create();
        var rows = await conn.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id, Price = price, UpdatedAt = updatedAt }, cancellationToken: ct));

        return rows > 0;
    }
}
