using MediatR;
using OsService.Infrastructure.Repository;

namespace OsService.Services.V1.ServiceOrders;

public sealed class GetServiceOrderByIdHandler(IServiceOrderRepository repository)
    : IRequestHandler<GetServiceOrderByIdQuery, ServiceOrderDto?>
{
    public async Task<ServiceOrderDto?> Handle(GetServiceOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var serviceOrder = await repository.GetByIdAsync(request.ServiceOrderId, cancellationToken);

        if (serviceOrder is null)
            return null;

        return new ServiceOrderDto(
            serviceOrder.Id,
            serviceOrder.Number,
            serviceOrder.CustomerId,
            serviceOrder.CustomerName,  // ? NOVO
            serviceOrder.Description,
            serviceOrder.Status,
            serviceOrder.OpenedAt,
            serviceOrder.StartedAt,
            serviceOrder.FinishedAt,
            serviceOrder.Price,
            serviceOrder.Coin,
            serviceOrder.UpdatedPriceAt
        );
    }
}
