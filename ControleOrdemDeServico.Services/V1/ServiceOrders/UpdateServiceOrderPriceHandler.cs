using MediatR;
using Microsoft.Extensions.Logging;
using OsService.Domain.Enums;
using OsService.Infrastructure.Repository;

namespace OsService.Services.V1.ServiceOrders;

public sealed class UpdateServiceOrderPriceHandler(
    IServiceOrderRepository repository,
    ILogger<UpdateServiceOrderPriceHandler> logger
) : IRequestHandler<UpdateServiceOrderPriceCommand, bool>
{
    public async Task<bool> Handle(UpdateServiceOrderPriceCommand request, CancellationToken ct)
    {
        if (request.ServiceOrderId == Guid.Empty)
            throw new ArgumentException("ServiceOrderId is required.");

        if (request.Price < 0)
            throw new ArgumentException("Price cannot be negative.");

        var serviceOrder = await repository.GetByIdAsync(request.ServiceOrderId, ct);

        if (serviceOrder is null)
            return false;

        // Cannot change price of finished order
        if (serviceOrder.Status == ServiceOrderStatus.Finished)
            throw new InvalidOperationException("Cannot change price of a finished Service Order.");

        var success = await repository.UpdatePriceAsync(
            request.ServiceOrderId,
            request.Price,
            DateTime.UtcNow,
            ct);

        if (success)
        {
            logger.LogInformation(
                "Service Order {ServiceOrderId} price updated to {Price}",
                request.ServiceOrderId,
                request.Price);
        }

        return success;
    }
}
