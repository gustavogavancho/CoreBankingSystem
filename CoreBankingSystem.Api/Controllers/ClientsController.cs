using CoreBankingSystem.Application.Accounts.Models;
using CoreBankingSystem.Application.Accounts.Queries;
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
    public async Task<ActionResult<List<ClientDto>>> Get(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetClientsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClientDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetClientByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{clientId:guid}/accounts")]
    public async Task<ActionResult<List<AccountDto>>> GetClientAccounts(Guid clientId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAccountsByClientQuery(clientId), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ClientDto>> Create(CreateClientCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ClientDto>> Update(Guid id, [FromBody] UpdateClientCommand command, CancellationToken cancellationToken)
    {
        // Enforce id from route so clients don't need to send it in the body
        if (id != command.Id)
        {
            command = command with { Id = id };
        }
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteClientCommand(id), cancellationToken);
        return NoContent();
    }
}
