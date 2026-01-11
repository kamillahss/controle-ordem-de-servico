using MediatR;
using OsService.Domain.Enums;

namespace OsService.Services.V1.ServiceOrders;

public sealed record UpdateServiceOrderStatusCommand(
    Guid ServiceOrderId,
    ServiceOrderStatus NewStatus
) : IRequest<bool>;
