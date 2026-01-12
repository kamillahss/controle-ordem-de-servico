using OsService.Domain.Entities;
using OsService.Infrastructure.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace OsService.Services.V1.Customers.CreateCustomer;

public sealed class CreateCustomerHandler(
    ICustomerRepository repository,
    ILogger<CreateCustomerHandler> logger
) : IRequestHandler<CreateCustomerCommand, Guid>
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Nome é obrigatório.");

        if (request.Name.Length < 2 || request.Name.Length > 150)
            throw new ArgumentException("Nome deve ter entre 2 e 150 caracteres.");

        var email = request.Email?.Trim();
        if (!string.IsNullOrEmpty(email) && !EmailRegex.IsMatch(email))
            throw new ArgumentException("Formato de e-mail inválido.");

        var customer = new CustomerEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Phone = request.Phone?.Trim(),
            Email = email,
            Document = request.Document?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await repository.InsertAsync(customer, ct);

        logger.LogInformation("Cliente {CustomerId} criado com nome {CustomerName}", customer.Id, customer.Name);

        return customer.Id;
    }
}
