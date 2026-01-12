using OsService.Services.V1.ServiceOrders.OpenServiceOrder;
using OsService.Services.V1.ServiceOrders.GetAllServiceOrders;
using OsService.Services.V1.ServiceOrders.GetServiceOrderById;
using OsService.Services.V1.ServiceOrders.UpdateServiceOrderPrice;
using OsService.Services.V1.ServiceOrders.UpdateServiceOrderStatus;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace OsService.ApiService.Controllers;

[ApiController]
[Route("api/v1/serviceorders")]
public sealed class ServiceOrdersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var serviceOrders = await mediator.Send(new GetAllServiceOrdersQuery(), ct);
        return Ok(serviceOrders);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var serviceOrder = await mediator.Send(new GetServiceOrderByIdQuery(id), ct);

        if (serviceOrder is null)
            return NotFound(new { error = "Ordem de serviço não encontrada" });

        return Ok(serviceOrder);
    }

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

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateServiceOrderStatusCommand cmd, CancellationToken ct)
    {
        if (id != cmd.ServiceOrderId)
            return BadRequest(new { error = "ID da ordem de serviço não corresponde" });

        try
        {
            var success = await mediator.Send(cmd, ct);

            if (!success)
                return NotFound(new { error = "Ordem de serviço não encontrada" });

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

    [HttpPut("{id:guid}/price")]
    public async Task<IActionResult> UpdatePrice(Guid id, [FromBody] UpdateServiceOrderPriceCommand cmd, CancellationToken ct)
    {
        if (id != cmd.ServiceOrderId)
            return BadRequest(new { error = "ID da ordem de serviço não corresponde" });

        try
        {
            var success = await mediator.Send(cmd, ct);

            if (!success)
                return NotFound(new { error = "Ordem de serviço não encontrada" });

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

