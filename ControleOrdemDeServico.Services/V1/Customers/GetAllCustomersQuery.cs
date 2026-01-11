using MediatR;

namespace OsService.Services.V1.Customers;

/// <summary>
/// Query para buscar todos os clientes cadastrados no sistema.
/// </summary>
public sealed record GetAllCustomersQuery() : IRequest<IEnumerable<CustomerDto>>;
