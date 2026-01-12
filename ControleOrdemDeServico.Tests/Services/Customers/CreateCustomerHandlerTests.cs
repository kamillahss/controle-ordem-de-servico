using Microsoft.Extensions.Logging;
using Moq;
using OsService.Domain.Entities;
using OsService.Infrastructure.Repository;
using OsService.Services.V1.Customers.CreateCustomer;

namespace OsService.Tests.Services.Customers;

public class CreateCustomerHandlerTests
{
    private readonly Mock<ICustomerRepository> _mockRepository;
    private readonly Mock<ILogger<CreateCustomerHandler>> _mockLogger;
    private readonly CreateCustomerHandler _handler;

    public CreateCustomerHandlerTests()
    {
        _mockRepository = new Mock<ICustomerRepository>();
        _mockLogger = new Mock<ILogger<CreateCustomerHandler>>();
        _handler = new CreateCustomerHandler(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ClienteValido_RetornaIdDoCliente()
    {
        // Arrange
        var command = new CreateCustomerCommand("Kamilla Sousa", "1234567890", "kamilla@example.com", "12345678901");
        var ct = CancellationToken.None;

        _mockRepository
            .Setup(r => r.InsertAsync(It.IsAny<CustomerEntity>(), ct))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, ct);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _mockRepository.Verify(r => r.InsertAsync(It.Is<CustomerEntity>(c =>
            c.Name == "Kamilla Sousa" &&
            c.Phone == "1234567890" &&
            c.Email == "kamilla@example.com" &&
            c.Document == "12345678901"
        ), ct), Times.Once);
    }

    [Fact]
    public async Task Handle_NomeVazio_LancaArgumentException()
    {
        // Arrange
        var command = new CreateCustomerCommand("", null, null, null);
        var ct = CancellationToken.None;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, ct));
    }

    [Fact]
    public async Task Handle_NomeComEspacosEmBranco_LancaArgumentException()
    {
        // Arrange
        var command = new CreateCustomerCommand("   ", null, null, null);
        var ct = CancellationToken.None;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, ct));
    }

    [Fact]
    public async Task Handle_NomeMuitoCurto_LancaArgumentException()
    {
        // Arrange
        var command = new CreateCustomerCommand("A", null, null, null);
        var ct = CancellationToken.None;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, ct));
    }

    [Fact]
    public async Task Handle_NomeMuitoLongo_LancaArgumentException()
    {
        // Arrange
        var longName = new string('A', 151);
        var command = new CreateCustomerCommand(longName, null, null, null);
        var ct = CancellationToken.None;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, ct));
    }

    [Fact]
    public async Task Handle_FormatoDeEmailInvalido_LancaArgumentException()
    {
        // Arrange
        var command = new CreateCustomerCommand("Kamilla Sousa", null, "invalid-email", null);
        var ct = CancellationToken.None;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, ct));
    }

    [Fact]
    public async Task Handle_FormatoDeEmailValido_Sucesso()
    {
        // Arrange
        var command = new CreateCustomerCommand("Kamilla Sousa", null, "kamilla@example.com", null);
        var ct = CancellationToken.None;

        _mockRepository
            .Setup(r => r.InsertAsync(It.IsAny<CustomerEntity>(), ct))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, ct);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
    }

    [Fact]
    public async Task Handle_RemoveEspacosEmBrancoDoCampos()
    {
        // Arrange
        var command = new CreateCustomerCommand("  Kamilla Sousa  ", "  1234567890  ", "  kamilla@example.com  ", "  12345678901  ");
        var ct = CancellationToken.None;

        _mockRepository
            .Setup(r => r.InsertAsync(It.IsAny<CustomerEntity>(), ct))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, ct);

        // Assert
        _mockRepository.Verify(r => r.InsertAsync(It.Is<CustomerEntity>(c =>
            c.Name == "Kamilla Sousa" &&
            c.Phone == "1234567890" &&
            c.Email == "kamilla@example.com" &&
            c.Document == "12345678901"
        ), ct), Times.Once);
    }
}
