using MediatR;
using OsService.Infrastructure.Repository;

namespace OsService.Services.V1.ServiceOrders;

public sealed class GetAllServiceOrdersHandler(IServiceOrderRepository repository)
    : IRequestHandler<GetAllServiceOrdersQuery, IEnumerable<ServiceOrderDto>>
{
    public async Task<IEnumerable<ServiceOrderDto>> Handle(GetAllServiceOrdersQuery request, CancellationToken cancellationToken)
    {
        var serviceOrders = await repository.GetAllAsync(cancellationToken);

        return serviceOrders.Select(so => new ServiceOrderDto(
            so.Id,
            so.Number,
            so.CustomerId,
            so.CustomerName,
            so.Description,
            so.Status,
            so.OpenedAt,
            so.StartedAt,
            so.FinishedAt,
            so.Price,
            so.Coin,
            so.UpdatedPriceAt
        ));
    }
}


