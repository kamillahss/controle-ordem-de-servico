using OsService.Domain.Entities;

namespace OsService.Infrastructure.Repository;

public interface IServiceOrderRepository
{
    Task<(Guid Id, int Number)> InsertAsync(ServiceOrderEntity serviceOrder, CancellationToken ct);
    Task<ServiceOrderEntity?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<ServiceOrderEntity>> GetAllAsync(CancellationToken ct);
    Task<bool> UpdateStatusAsync(Guid id, int status, DateTime? startedAt, DateTime? finishedAt, CancellationToken ct);
    Task<bool> UpdatePriceAsync(Guid id, decimal price, DateTime updatedAt, CancellationToken ct);
}

