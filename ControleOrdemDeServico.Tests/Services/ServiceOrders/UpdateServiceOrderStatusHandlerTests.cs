using Microsoft.Extensions.Logging;
using Moq;
using OsService.Domain.Entities;
using OsService.Domain.Enums;
using OsService.Infrastructure.Repository;
using OsService.Services.V1.ServiceOrders;
using Xunit;

namespace OsService.Tests.Services.ServiceOrders;

public class UpdateServiceOrderStatusHandlerTests
{
    private readonly Mock<IServiceOrderRepository> _mockRepository;
    private readonly Mock<ILogger<UpdateServiceOrderStatusHandler>> _mockLogger;
    private readonly UpdateServiceOrderStatusHandler _handler;

    public UpdateServiceOrderStatusHandlerTests()
    {
        _mockRepository = new Mock<IServiceOrderRepository>();
        _mockLogger = new Mock<ILogger<UpdateServiceOrderStatusHandler>>();
        _handler = new UpdateServiceOrderStatusHandler(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_AbertaParaEmAndamento_Sucesso()
    {
        // Arrange
        var serviceOrderId = Guid.NewGuid();
        var command = new UpdateServiceOrderStatusCommand(serviceOrderId, ServiceOrderStatus.InProgress);
        var ct = CancellationToken.None;

        var existingOrder = new ServiceOrderEntity
        {
            Id = serviceOrderId,
            Number = 1001,
            CustomerId = Guid.NewGuid(),
            Description = "Test",
            Status = ServiceOrderStatus.Open,
            OpenedAt = DateTime.UtcNow,
            Coin = "BRL"
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(serviceOrderId, ct))
            .ReturnsAsync(existingOrder);

        _mockRepository
            .Setup(r => r.UpdateStatusAsync(serviceOrderId, (int)ServiceOrderStatus.InProgress, It.IsAny<DateTime>(), null, ct))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, ct);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.UpdateStatusAsync(
            serviceOrderId,
            (int)ServiceOrderStatus.InProgress,
            It.Is<DateTime?>(dt => dt.HasValue),
            null,
            ct), Times.Once);
    }

    [Fact]
    public async Task Handle_EmAndamentoParaFinalizada_Sucesso()
    {
        // Arrange
        var serviceOrderId = Guid.NewGuid();
        var command = new UpdateServiceOrderStatusCommand(serviceOrderId, ServiceOrderStatus.Finished);
        var ct = CancellationToken.None;
        var startedAt = DateTime.UtcNow.AddHours(-2);

        var existingOrder = new ServiceOrderEntity
        {
            Id = serviceOrderId,
            Number = 1001,
            CustomerId = Guid.NewGuid(),
            Description = "Test",
            Status = ServiceOrderStatus.InProgress,
            OpenedAt = DateTime.UtcNow.AddHours(-3),
            StartedAt = startedAt,
            Coin = "BRL"
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(serviceOrderId, ct))
            .ReturnsAsync(existingOrder);

        _mockRepository
            .Setup(r => r.UpdateStatusAsync(serviceOrderId, (int)ServiceOrderStatus.Finished, startedAt, It.IsAny<DateTime>(), ct))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, ct);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.UpdateStatusAsync(
            serviceOrderId,
            (int)ServiceOrderStatus.Finished,
            startedAt,
            It.Is<DateTime?>(dt => dt.HasValue),
            ct), Times.Once);
    }

    [Fact]
    public async Task Handle_AbertaParaFinalizada_LancaInvalidOperationException()
    {
        // Arrange
        var serviceOrderId = Guid.NewGuid();
        var command = new UpdateServiceOrderStatusCommand(serviceOrderId, ServiceOrderStatus.Finished);
        var ct = CancellationToken.None;

        var existingOrder = new ServiceOrderEntity
        {
            Id = serviceOrderId,
            Number = 1001,
            CustomerId = Guid.NewGuid(),
            Description = "Test",
            Status = ServiceOrderStatus.Open,
            OpenedAt = DateTime.UtcNow,
            Coin = "BRL"
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(serviceOrderId, ct))
            .ReturnsAsync(existingOrder);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, ct));
    }

    [Fact]
    public async Task Handle_FinalizadaParaQualquerStatus_LancaInvalidOperationException()
    {
        // Arrange
        var serviceOrderId = Guid.NewGuid();
        var command = new UpdateServiceOrderStatusCommand(serviceOrderId, ServiceOrderStatus.Open);
        var ct = CancellationToken.None;

        var existingOrder = new ServiceOrderEntity
        {
            Id = serviceOrderId,
            Number = 1001,
            CustomerId = Guid.NewGuid(),
            Description = "Test",
            Status = ServiceOrderStatus.Finished,
            OpenedAt = DateTime.UtcNow.AddHours(-3),
            StartedAt = DateTime.UtcNow.AddHours(-2),
            FinishedAt = DateTime.UtcNow.AddHours(-1),
            Coin = "BRL"
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(serviceOrderId, ct))
            .ReturnsAsync(existingOrder);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, ct));
    }

    [Fact]
    public async Task Handle_OrdemDeServicoNaoEncontrada_RetornaFalso()
    {
        // Arrange
        var serviceOrderId = Guid.NewGuid();
        var command = new UpdateServiceOrderStatusCommand(serviceOrderId, ServiceOrderStatus.InProgress);
        var ct = CancellationToken.None;

        _mockRepository
            .Setup(r => r.GetByIdAsync(serviceOrderId, ct))
            .ReturnsAsync((ServiceOrderEntity?)null);

        // Act
        var result = await _handler.Handle(command, ct);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Handle_IdOrdemDeServicoVazio_LancaArgumentException()
    {
        // Arrange
        var command = new UpdateServiceOrderStatusCommand(Guid.Empty, ServiceOrderStatus.InProgress);
        var ct = CancellationToken.None;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, ct));
    }
}
