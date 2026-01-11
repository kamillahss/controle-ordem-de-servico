using MediatR;

namespace OsService.Services.V1.ServiceOrders;

public sealed record GetAllServiceOrdersQuery() : IRequest<IEnumerable<ServiceOrderDto>>;
