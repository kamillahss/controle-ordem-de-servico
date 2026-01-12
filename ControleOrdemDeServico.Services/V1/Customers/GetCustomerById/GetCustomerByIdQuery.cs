using MediatR;

namespace OsService.Services.V1.Customers.GetCustomerById;

public sealed record GetCustomerByIdQuery(Guid CustomerId) : IRequest<CustomerDto?>;

public sealed record CustomerDto(
    Guid Id,
    string Name,
    string? Phone,
    string? Email,
    string? Document,
    DateTime CreatedAt
);
