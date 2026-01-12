using MediatR;
using Microsoft.Extensions.Logging;
using OsService.Domain.Enums;
using OsService.Infrastructure.Repository;

namespace OsService.Services.V1.ServiceOrders.UpdateServiceOrderPrice;

public sealed class UpdateServiceOrderPriceHandler(
    IServiceOrderRepository repository,
    ILogger<UpdateServiceOrderPriceHandler> logger
) : IRequestHandler<UpdateServiceOrderPriceCommand, bool>
{
    public async Task<bool> Handle(UpdateServiceOrderPriceCommand request, CancellationToken cancellationToken)
    {
        if (request.ServiceOrderId == Guid.Empty)
            throw new ArgumentException("ServiceOrderId é obrigatório.");

        if (request.Price < 0)
            throw new ArgumentException("Preço não pode ser negativo.");

        var serviceOrder = await repository.GetByIdAsync(request.ServiceOrderId, cancellationToken);

        if (serviceOrder is null)
            return false;

        if (serviceOrder.Status == ServiceOrderStatus.Finished)
            throw new InvalidOperationException("Não é possível alterar o preço de uma ordem de serviço finalizada.");

        var success = await repository.UpdatePriceAsync(
            request.ServiceOrderId,
            request.Price,
            DateTime.UtcNow,
            cancellationToken);

        if (success)
        {
            logger.LogInformation(
                "Ordem de Serviço {ServiceOrderId} preço atualizado para {Price}",
                request.ServiceOrderId,
                request.Price);
        }

        return success;
    }
}
