using MediatR;
using OsService.Services.V1.Customers.GetCustomerById;

namespace OsService.Services.V1.Customers.GetAllCustomers;

#pragma warning disable S2094
public sealed record GetAllCustomersQuery : IRequest<IEnumerable<CustomerDto>>;
#pragma warning restore S2094
