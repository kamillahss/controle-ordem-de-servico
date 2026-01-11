using MediatR;

namespace OsService.Services.V1.Customers;

public sealed record GetAllCustomersQuery() : IRequest<IEnumerable<CustomerDto>>;
