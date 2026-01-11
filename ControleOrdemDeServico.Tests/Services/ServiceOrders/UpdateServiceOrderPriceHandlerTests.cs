using Microsoft.Extensions.Logging;
using Moq;
using OsService.Domain.Entities;
using OsService.Domain.Enums;
using OsService.Infrastructure.Repository;
using OsService.Services.V1.ServiceOrders;
using Xunit;

namespace OsService.Tests.Services.ServiceOrders;

public class UpdateServiceOrderPriceHandlerTests
{
    private readonly Mock<IServiceOrderRepository> _mockRepository;
    private readonly Mock<ILogger<UpdateServiceOrderPriceHandler>> _mockLogger;
    private readonly UpdateServiceOrderPriceHandler _handler;

    public UpdateServiceOrderPriceHandlerTests()
    {
        _mockRepository = new Mock<IServiceOrderRepository>();
        _mockLogger = new Mock<ILogger<UpdateServiceOrderPriceHandler>>();
        _handler = new UpdateServiceOrderPriceHandler(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_PrecoValido_Sucesso()
    {
        // Arrange
        var serviceOrderId = Guid.NewGuid();
        var command = new UpdateServiceOrderPriceCommand(serviceOrderId, 150.50m);
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
            .Setup(r => r.UpdatePriceAsync(serviceOrderId, 150.50m, It.IsAny<DateTime>(), ct))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, ct);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.UpdatePriceAsync(
            serviceOrderId,
            150.50m,
            It.IsAny<DateTime>(),
            ct), Times.Once);
    }

    [Fact]
    public async Task Handle_PrecoNegativo_LancaArgumentException()
    {
        // Arrange
        var serviceOrderId = Guid.NewGuid();
        var command = new UpdateServiceOrderPriceCommand(serviceOrderId, -10);
        var ct = CancellationToken.None;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, ct));
    }

    [Fact]
    public async Task Handle_OrdemDeServicoFinalizada_LancaInvalidOperationException()
    {
        // Arrange
        var serviceOrderId = Guid.NewGuid();
        var command = new UpdateServiceOrderPriceCommand(serviceOrderId, 150.50m);
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
        var command = new UpdateServiceOrderPriceCommand(serviceOrderId, 150.50m);
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
        var command = new UpdateServiceOrderPriceCommand(Guid.Empty, 150.50m);
        var ct = CancellationToken.None;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, ct));
    }

    [Fact]
    public async Task Handle_PrecoZero_Sucesso()
    {
        // Arrange
        var serviceOrderId = Guid.NewGuid();
        var command = new UpdateServiceOrderPriceCommand(serviceOrderId, 0);
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
            .Setup(r => r.UpdatePriceAsync(serviceOrderId, 0, It.IsAny<DateTime>(), ct))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, ct);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task Handle_AtualizarPrecoEmAndamento_Sucesso()
    {
        // Arrange
        var serviceOrderId = Guid.NewGuid();
        var command = new UpdateServiceOrderPriceCommand(serviceOrderId, 200.00m);
        var ct = CancellationToken.None;

        var existingOrder = new ServiceOrderEntity
        {
            Id = serviceOrderId,
            Number = 1001,
            CustomerId = Guid.NewGuid(),
            Description = "Test",
            Status = ServiceOrderStatus.InProgress,
            OpenedAt = DateTime.UtcNow.AddHours(-1),
            StartedAt = DateTime.UtcNow,
            Coin = "BRL"
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(serviceOrderId, ct))
            .ReturnsAsync(existingOrder);

        _mockRepository
            .Setup(r => r.UpdatePriceAsync(serviceOrderId, 200.00m, It.IsAny<DateTime>(), ct))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, ct);

        // Assert
        Assert.True(result);
    }
}
