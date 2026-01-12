using MediatR;
using OsService.Domain.Enums;

namespace OsService.Services.V1.ServiceOrders.UpdateServiceOrderStatus;

public sealed record UpdateServiceOrderStatusCommand(
    Guid ServiceOrderId,
    ServiceOrderStatus NewStatus
) : IRequest<bool>;
