using OsService.Domain.Entities;
using OsService.Domain.Enums;
using OsService.Infrastructure.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace OsService.Services.V1.ServiceOrders.OpenServiceOrder;

public sealed class OpenServiceOrderHandler(
    ICustomerRepository customerRepository,
    IServiceOrderRepository serviceOrderRepository,
    ILogger<OpenServiceOrderHandler> logger
) : IRequestHandler<OpenServiceOrderCommand, (Guid Id, int Number)>
{
    public async Task<(Guid Id, int Number)> Handle(OpenServiceOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.CustomerId == Guid.Empty)
            throw new ArgumentException("CustomerId é obrigatório.");

        if (string.IsNullOrWhiteSpace(request.Description))
            throw new ArgumentException("Descrição é obrigatória.");

        if (request.Description.Length > 500)
            throw new ArgumentException("Descrição deve ter no máximo 500 caracteres.");

        var customerExists = await customerRepository.ExistsAsync(request.CustomerId, cancellationToken);
        if (!customerExists)
            throw new KeyNotFoundException($"Cliente com ID {request.CustomerId} não encontrado.");

        var serviceOrder = new ServiceOrderEntity
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            Description = request.Description.Trim(),
            Status = ServiceOrderStatus.Open,
            OpenedAt = DateTime.UtcNow,
            Coin = "BRL"
        };

        var (id, number) = await serviceOrderRepository.InsertAsync(serviceOrder, cancellationToken);

        logger.LogInformation("Ordem de Serviço {Number} criada para Cliente {CustomerId}", number, request.CustomerId);

        return (id, number);
    }
}
