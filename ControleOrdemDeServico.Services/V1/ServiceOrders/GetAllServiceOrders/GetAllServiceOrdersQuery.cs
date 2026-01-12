using MediatR;
using OsService.Services.V1.ServiceOrders.GetServiceOrderById;

namespace OsService.Services.V1.ServiceOrders.GetAllServiceOrders;

#pragma warning disable S2094
public sealed record GetAllServiceOrdersQuery : IRequest<IEnumerable<ServiceOrderDto>>;
#pragma warning restore S2094
