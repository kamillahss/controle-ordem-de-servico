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
            throw new ArgumentException("ServiceOrderId is required.");

        var serviceOrder = await repository.GetByIdAsync(request.ServiceOrderId, ct);

        if (serviceOrder is null)
            return false;

        // Validate status transitions
        ValidateStatusTransition(serviceOrder.Status, request.NewStatus);

        // Cannot change status of finished order
        if (serviceOrder.Status == ServiceOrderStatus.Finished)
            throw new InvalidOperationException("Cannot change status of a finished Service Order.");

        DateTime? startedAt = serviceOrder.StartedAt;
        DateTime? finishedAt = serviceOrder.FinishedAt;

        // Set StartedAt when moving to InProgress
        if (request.NewStatus == ServiceOrderStatus.InProgress && !startedAt.HasValue)
        {
            startedAt = DateTime.UtcNow;
        }

        // Set FinishedAt when moving to Finished
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
                "Service Order {ServiceOrderId} status changed from {OldStatus} to {NewStatus}",
                request.ServiceOrderId,
                serviceOrder.Status,
                request.NewStatus);
        }

        return success;
    }

    private static void ValidateStatusTransition(ServiceOrderStatus currentStatus, ServiceOrderStatus newStatus)
    {
        // Open -> InProgress (allowed)
        if (currentStatus == ServiceOrderStatus.Open && newStatus == ServiceOrderStatus.InProgress)
            return;

        // InProgress -> Finished (allowed)
        if (currentStatus == ServiceOrderStatus.InProgress && newStatus == ServiceOrderStatus.Finished)
            return;

        // Open -> Finished (blocked)
        if (currentStatus == ServiceOrderStatus.Open && newStatus == ServiceOrderStatus.Finished)
            throw new InvalidOperationException("Cannot change status from Open directly to Finished. Must go through InProgress first.");

        // Finished -> any (blocked)
        if (currentStatus == ServiceOrderStatus.Finished)
            throw new InvalidOperationException("Cannot change status of a finished Service Order.");

        // Any other invalid transition
        throw new InvalidOperationException($"Invalid status transition from {currentStatus} to {newStatus}.");
    }
}
