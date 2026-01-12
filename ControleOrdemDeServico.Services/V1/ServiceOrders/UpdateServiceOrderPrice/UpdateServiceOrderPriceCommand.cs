using MediatR;

namespace OsService.Services.V1.ServiceOrders.UpdateServiceOrderPrice;

public sealed record UpdateServiceOrderPriceCommand(
    Guid ServiceOrderId,
    decimal Price
) : IRequest<bool>;
