using Microsoft.Extensions.Logging;
using Moq;
using OsService.Domain.Entities;
using OsService.Domain.Enums;
using OsService.Infrastructure.Repository;
using OsService.Services.V1.ServiceOrders.OpenServiceOrder;

namespace OsService.Tests.Services.ServiceOrders;

public class OpenServiceOrderHandlerTests
{
    private readonly Mock<ICustomerRepository> _mockCustomerRepository;
    private readonly Mock<IServiceOrderRepository> _mockServiceOrderRepository;
    private readonly Mock<ILogger<OpenServiceOrderHandler>> _mockLogger;
    private readonly OpenServiceOrderHandler _handler;

    public OpenServiceOrderHandlerTests()
    {
        _mockCustomerRepository = new Mock<ICustomerRepository>();
        _mockServiceOrderRepository = new Mock<IServiceOrderRepository>();
        _mockLogger = new Mock<ILogger<OpenServiceOrderHandler>>();
        _handler = new OpenServiceOrderHandler(
            _mockCustomerRepository.Object,
            _mockServiceOrderRepository.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_OrdemDeServicoValida_RetornaIdENumero()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var command = new OpenServiceOrderCommand(customerId, "Fix computer");
        var ct = CancellationToken.None;
        var expectedId = Guid.NewGuid();
        var expectedNumber = 1001;

        _mockCustomerRepository
            .Setup(r => r.ExistsAsync(customerId, ct))
            .ReturnsAsync(true);

        _mockServiceOrderRepository
            .Setup(r => r.InsertAsync(It.IsAny<ServiceOrderEntity>(), ct))
            .ReturnsAsync((expectedId, expectedNumber));

        // Act
        var result = await _handler.Handle(command, ct);

        // Assert
        Assert.Equal(expectedId, result.Id);
        Assert.Equal(expectedNumber, result.Number);

        _mockServiceOrderRepository.Verify(r => r.InsertAsync(It.Is<ServiceOrderEntity>(so =>
            so.CustomerId == customerId &&
            so.Description == "Fix computer" &&
            so.Status == ServiceOrderStatus.Open &&
            so.Coin == "BRL"
        ), ct), Times.Once);
    }

    [Fact]
    public async Task Handle_IdClienteVazio_LancaArgumentException()
    {
        // Arrange
        var command = new OpenServiceOrderCommand(Guid.Empty, "Fix computer");
        var ct = CancellationToken.None;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, ct));
    }

    [Fact]
    public async Task Handle_DescricaoVazia_LancaArgumentException()
    {
        // Arrange
        var command = new OpenServiceOrderCommand(Guid.NewGuid(), "");
        var ct = CancellationToken.None;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, ct));
    }

    [Fact]
    public async Task Handle_DescricaoComEspacosEmBranco_LancaArgumentException()
    {
        // Arrange
        var command = new OpenServiceOrderCommand(Guid.NewGuid(), "   ");
        var ct = CancellationToken.None;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, ct));
    }

    [Fact]
    public async Task Handle_DescricaoMuitoLonga_LancaArgumentException()
    {
        // Arrange
        var longDescription = new string('A', 501);
        var command = new OpenServiceOrderCommand(Guid.NewGuid(), longDescription);
        var ct = CancellationToken.None;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, ct));
    }

    [Fact]
    public async Task Handle_ClienteNaoEncontrado_LancaKeyNotFoundException()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var command = new OpenServiceOrderCommand(customerId, "Fix computer");
        var ct = CancellationToken.None;

        _mockCustomerRepository
            .Setup(r => r.ExistsAsync(customerId, ct))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, ct));
    }

    [Fact]
    public async Task Handle_RemoveEspacosEmBrancoDaDescricao()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var command = new OpenServiceOrderCommand(customerId, "  Fix computer  ");
        var ct = CancellationToken.None;

        _mockCustomerRepository
            .Setup(r => r.ExistsAsync(customerId, ct))
            .ReturnsAsync(true);

        _mockServiceOrderRepository
            .Setup(r => r.InsertAsync(It.IsAny<ServiceOrderEntity>(), ct))
            .ReturnsAsync((Guid.NewGuid(), 1001));

        // Act
        await _handler.Handle(command, ct);

        // Assert
        _mockServiceOrderRepository.Verify(r => r.InsertAsync(It.Is<ServiceOrderEntity>(so =>
            so.Description == "Fix computer"
        ), ct), Times.Once);
    }
}
