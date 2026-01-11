using OsService.Services.V1.ServiceOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace OsService.ApiService.Controllers;

[ApiController]
[Route("api/v1/serviceorders")]
public sealed class ServiceOrdersController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Lista todas as ordens de serviço.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var serviceOrders = await mediator.Send(new GetAllServiceOrdersQuery(), ct);
        return Ok(serviceOrders);
    }

    /// <summary>
    /// Busca uma ordem de serviço específica por ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var serviceOrder = await mediator.Send(new GetServiceOrderByIdQuery(id), ct);

        if (serviceOrder is null)
            return NotFound(new { error = "Service Order not found" });

        return Ok(serviceOrder);
    }

    /// <summary>
    /// Abre uma nova ordem de serviço.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Open([FromBody] OpenServiceOrderCommand cmd, CancellationToken ct)
    {
        try
        {
            var (id, number) = await mediator.Send(cmd, ct);
            return CreatedAtAction(nameof(GetById), new { id }, new { id, number });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza o status de uma ordem de serviço.
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateServiceOrderStatusCommand cmd, CancellationToken ct)
    {
        if (id != cmd.ServiceOrderId)
            return BadRequest(new { error = "Service Order ID mismatch" });

        try
        {
            var success = await mediator.Send(cmd, ct);

            if (!success)
                return NotFound(new { error = "Service Order not found" });

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza o preço de uma ordem de serviço.
    /// </summary>
    [HttpPut("{id:guid}/price")]
    public async Task<IActionResult> UpdatePrice(Guid id, [FromBody] UpdateServiceOrderPriceCommand cmd, CancellationToken ct)
    {
        if (id != cmd.ServiceOrderId)
            return BadRequest(new { error = "Service Order ID mismatch" });

        try
        {
            var success = await mediator.Send(cmd, ct);

            if (!success)
                return NotFound(new { error = "Service Order not found" });

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

