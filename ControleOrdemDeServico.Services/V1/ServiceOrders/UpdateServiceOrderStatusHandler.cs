using MediatR;
using Microsoft.Extensions.Logging;
using OsService.Domain.Enums;
using OsService.Infrastructure.Repository;

namespace OsService.Services.V1.ServiceOrders;

public sealed class UpdateServiceOrderStatusHandler(
    IServiceOrderRepository repository,
    ILogger<UpdateServiceOrderStatusHandler> logger
) : IRequestHandler<UpdateServiceOrderStatusCommand, bool>
{
    public async Task<bool> Handle(UpdateServiceOrderStatusCommand request, CancellationToken ct)
    {
        if (request.ServiceOrderId == Guid.Empty)
            throw new ArgumentException("ServiceOrderId é obrigatório.");

        var serviceOrder = await repository.GetByIdAsync(request.ServiceOrderId, ct);

        if (serviceOrder is null)
            return false;

        ValidateStatusTransition(serviceOrder.Status, request.NewStatus);

        if (serviceOrder.Status == ServiceOrderStatus.Finished)
            throw new InvalidOperationException("Não é possível alterar o status de uma ordem de serviço finalizada.");

        DateTime? startedAt = serviceOrder.StartedAt;
        DateTime? finishedAt = serviceOrder.FinishedAt;

        if (request.NewStatus == ServiceOrderStatus.InProgress && !startedAt.HasValue)
        {
            startedAt = DateTime.UtcNow;
        }

        if (request.NewStatus == ServiceOrderStatus.Finished)
        {
            finishedAt = DateTime.UtcNow;
        }

        var success = await repository.UpdateStatusAsync(
            request.ServiceOrderId,
            (int)request.NewStatus,
            startedAt,
            finishedAt,
            ct);

        if (success)
        {
            logger.LogInformation(
                "Ordem de Serviço {ServiceOrderId} status alterado de {OldStatus} para {NewStatus}",
                request.ServiceOrderId,
                serviceOrder.Status,
                request.NewStatus);
        }

        return success;
    }

    private static void ValidateStatusTransition(ServiceOrderStatus currentStatus, ServiceOrderStatus newStatus)
    {
        if (currentStatus == ServiceOrderStatus.Open && newStatus == ServiceOrderStatus.InProgress)
            return;

        if (currentStatus == ServiceOrderStatus.InProgress && newStatus == ServiceOrderStatus.Finished)
            return;

        if (currentStatus == ServiceOrderStatus.Open && newStatus == ServiceOrderStatus.Finished)
            throw new InvalidOperationException("Não é possível alterar o status de Aberta diretamente para Finalizada. Deve passar por Em Andamento primeiro.");

        if (currentStatus == ServiceOrderStatus.Finished)
            throw new InvalidOperationException("Não é possível alterar o status de uma ordem de serviço finalizada.");

        throw new InvalidOperationException($"Transição de status inválida de {currentStatus} para {newStatus}.");
    }
}
