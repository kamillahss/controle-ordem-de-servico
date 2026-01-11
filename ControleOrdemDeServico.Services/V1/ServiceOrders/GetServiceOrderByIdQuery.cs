using MediatR;
using OsService.Domain.Enums;

namespace OsService.Services.V1.ServiceOrders;

public sealed record GetServiceOrderByIdQuery(Guid ServiceOrderId) : IRequest<ServiceOrderDto?>;

public sealed record ServiceOrderDto(
    Guid Id,
    int Number,
    Guid CustomerId,
    string CustomerName,  // ? NOVO
    string Description,
    ServiceOrderStatus Status,
    DateTime OpenedAt,
    DateTime? StartedAt,
    DateTime? FinishedAt,
    decimal? Price,
    string Coin,
    DateTime? UpdatedPriceAt
);

