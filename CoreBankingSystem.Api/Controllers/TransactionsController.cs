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
    public async Task<ActionResult<List<TransactionDto>>> Get(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetTransactionsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TransactionDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetTransactionByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    // Allow listing transactions by account number via a top-level explicit route
    [HttpGet]
    [Route("/api/accounts/{accountNumber}/transactions")]
    public async Task<ActionResult<List<TransactionDto>>> GetByAccount(string accountNumber, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetTransactionsByAccountNumberQuery(accountNumber), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> Create(CreateTransactionCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.TransactionId }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TransactionDto>> Update(Guid id, UpdateTransactionCommand command, CancellationToken cancellationToken)
    {
        if (id != command.TransactionId) return BadRequest();
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteTransactionCommand(id), cancellationToken);
        return NoContent();
    }
}
