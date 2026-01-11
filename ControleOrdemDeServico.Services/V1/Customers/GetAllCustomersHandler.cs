using MediatR;
using OsService.Infrastructure.Repository;

namespace OsService.Services.V1.Customers;

/// <summary>
/// Handler responsável por buscar todos os clientes cadastrados.
/// </summary>
public sealed class GetAllCustomersHandler(ICustomerRepository repository) 
    : IRequestHandler<GetAllCustomersQuery, IEnumerable<CustomerDto>>
{
    public async Task<IEnumerable<CustomerDto>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await repository.GetAllAsync(cancellationToken);
        
        return customers.Select(c => new CustomerDto(
            c.Id,
            c.Name,
            c.Phone,
            c.Email,
            c.Document,
            c.CreatedAt
        ));
    }
}
