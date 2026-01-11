using MediatR;

namespace OsService.Services.V1.ServiceOrders;

/// <summary>
/// Query para buscar todas as ordens de serviço cadastradas.
/// </summary>
public sealed record GetAllServiceOrdersQuery() : IRequest<IEnumerable<ServiceOrderDto>>;
