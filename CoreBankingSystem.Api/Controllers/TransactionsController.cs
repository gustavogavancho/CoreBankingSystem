using CoreBankingSystem.Application.Transactions.Commands;
using CoreBankingSystem.Application.Transactions.Models;
using CoreBankingSystem.Application.Transactions.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CoreBankingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TransactionDto>>> Get()
    {
        var result = await mediator.Send(new GetTransactionsQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TransactionDto>> GetById(Guid id)
    {
        var result = await mediator.Send(new GetTransactionByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> Create(CreateTransactionCommand command)
    {
        var result = await mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.TransactionId }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TransactionDto>> Update(Guid id, UpdateTransactionCommand command)
    {
        if (id != command.TransactionId) return BadRequest();
        var result = await mediator.Send(command);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var success = await mediator.Send(new DeleteTransactionCommand(id));
        return success ? NoContent() : NotFound();
    }
}
