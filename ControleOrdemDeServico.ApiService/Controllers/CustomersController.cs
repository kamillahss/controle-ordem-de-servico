using OsService.Services.V1.Customers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace OsService.ApiService.Controllers;

[ApiController]
[Route("api/v1/customers")]
public sealed class CustomersController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Lista todos os clientes cadastrados.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var customers = await mediator.Send(new GetAllCustomersQuery(), ct);
        return Ok(customers);
    }

    /// <summary>
    /// Busca um cliente específico por ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var customer = await mediator.Send(new GetCustomerByIdQuery(id), ct);

        if (customer is null)
            return NotFound(new { error = "Customer not found" });

        return Ok(customer);
    }

    /// <summary>
    /// Cria um novo cliente.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand cmd, CancellationToken ct)
    {
        try
        {
            var id = await mediator.Send(cmd, ct);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
