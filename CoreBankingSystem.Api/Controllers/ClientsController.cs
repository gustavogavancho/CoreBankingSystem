using CoreBankingSystem.Application.Clients.Commands;
using CoreBankingSystem.Application.Clients.Models;
using CoreBankingSystem.Application.Clients.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CoreBankingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ClientDto>>> Get()
    {
        var result = await mediator.Send(new GetClientsQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClientDto>> GetById(Guid id)
    {
        var result = await mediator.Send(new GetClientByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("by-client-id/{clientId:guid}")]
    public async Task<ActionResult<ClientDto>> GetByClientId(Guid clientId)
    {
        var result = await mediator.Send(new GetClientByClientIdQuery(clientId));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ClientDto>> Create(CreateClientCommand command)
    {
        var result = await mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ClientDto>> Update(Guid id, UpdateClientCommand command)
    {
        if (id != command.Id) return BadRequest();
        var result = await mediator.Send(command);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var success = await mediator.Send(new DeleteClientCommand(id));
        return success ? NoContent() : NotFound();
    }
}
