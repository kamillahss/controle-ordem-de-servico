using MediatR;
using OsService.Infrastructure.Repository;

namespace OsService.Services.V1.Customers;

public sealed class GetCustomerByIdHandler(ICustomerRepository repository)
    : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
{
    public async Task<CustomerDto?> Handle(GetCustomerByIdQuery request, CancellationToken ct)
    {
        var customer = await repository.GetByIdAsync(request.CustomerId, ct);

        if (customer is null)
            return null;

        return new CustomerDto(
            customer.Id,
            customer.Name,
            customer.Phone,
            customer.Email,
            customer.Document,
            customer.CreatedAt
        );
    }
}
