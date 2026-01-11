using OsService.Domain.Entities;
using OsService.Domain.Enums;
using OsService.Infrastructure.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace OsService.Services.V1.ServiceOrders;

public sealed class OpenServiceOrderHandler(
    ICustomerRepository customerRepository,
    IServiceOrderRepository serviceOrderRepository,
    ILogger<OpenServiceOrderHandler> logger
) : IRequestHandler<OpenServiceOrderCommand, (Guid Id, int Number)>
{
    public async Task<(Guid Id, int Number)> Handle(OpenServiceOrderCommand request, CancellationToken ct)
    {
        if (request.CustomerId == Guid.Empty)
            throw new ArgumentException("CustomerId is required.");

        if (string.IsNullOrWhiteSpace(request.Description))
            throw new ArgumentException("Description is required.");

        if (request.Description.Length > 500)
            throw new ArgumentException("Description must be <= 500 characters.");

        var customerExists = await customerRepository.ExistsAsync(request.CustomerId, ct);
        if (!customerExists)
            throw new KeyNotFoundException($"Customer with ID {request.CustomerId} not found.");

        var serviceOrder = new ServiceOrderEntity
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            Description = request.Description.Trim(),
            Status = ServiceOrderStatus.Open,
            OpenedAt = DateTime.UtcNow,
            Coin = "BRL"
        };

        var (id, number) = await serviceOrderRepository.InsertAsync(serviceOrder, ct);

        logger.LogInformation("Service Order {Number} created for Customer {CustomerId}", number, request.CustomerId);

        return (id, number);
    }
}
