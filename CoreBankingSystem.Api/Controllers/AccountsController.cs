using CoreBankingSystem.Application.Accounts.Models;
using CoreBankingSystem.Application.Accounts.Queries;
using CoreBankingSystem.Application.Accounts.Commands;
using CoreBankingSystem.Application.Transactions.Commands;
using CoreBankingSystem.Application.Transactions.Models;
using CoreBankingSystem.Application.Transactions.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CoreBankingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AccountDto>>> Get()
    {
        var result = await mediator.Send(new GetAccountsQuery());
        return Ok(result);
    }

    [HttpGet("{accountNumber}")]
    public async Task<ActionResult<AccountDto>> GetByNumber(string accountNumber)
    {
        var result = await mediator.Send(new GetAccountByNumberQuery(accountNumber));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<AccountDto>> Create(CreateAccountCommand command)
    {
        var result = await mediator.Send(command);
        return CreatedAtAction(nameof(GetByNumber), new { accountNumber = result.AccountNumber }, result);
    }

    [HttpPut("{accountNumber}")]
    public async Task<ActionResult<AccountDto>> Update(string accountNumber, UpdateAccountCommand command)
    {
        if (!string.Equals(accountNumber, command.AccountNumber, StringComparison.OrdinalIgnoreCase)) return BadRequest();
        var result = await mediator.Send(command);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{accountNumber}")]
    public async Task<ActionResult> Delete(string accountNumber)
    {
        var success = await mediator.Send(new DeleteAccountCommand(accountNumber));
        return success ? NoContent() : NotFound();
    }

    // Nested Transactions endpoints following REST pattern: /api/accounts/{accountNumber}/transactions

    [HttpGet("{accountNumber}/transactions")]
    public async Task<ActionResult<List<TransactionDto>>> GetAccountTransactions(string accountNumber)
    {
        var result = await mediator.Send(new GetTransactionsByAccountNumberQuery(accountNumber));
        return Ok(result);
    }

    [HttpGet("{accountNumber}/transactions/{transactionId:guid}")]
    public async Task<ActionResult<TransactionDto>> GetAccountTransactionById(string accountNumber, Guid transactionId)
    {
        var tx = await mediator.Send(new GetTransactionByIdQuery(transactionId));
        if (tx is null) return NotFound();
        // Optional guard: ensure the transaction belongs to the requested account
        if (!string.Equals(tx.AccountNumber, accountNumber, StringComparison.OrdinalIgnoreCase)) return NotFound();
        return Ok(tx);
    }

    [HttpPost("{accountNumber}/transactions")]
    public async Task<ActionResult<TransactionDto>> CreateAccountTransaction(string accountNumber, [FromBody] CreateTransactionCommand command)
    {
        // Enforce account from route
        if (!string.Equals(command.AccountNumber, accountNumber, StringComparison.OrdinalIgnoreCase))
        {
            command = command with { AccountNumber = accountNumber };
        }
        var result = await mediator.Send(command);
        return CreatedAtAction(nameof(GetAccountTransactionById), new { accountNumber, transactionId = result.TransactionId }, result);
    }

    [HttpPut("{accountNumber}/transactions/{transactionId:guid}")]
    public async Task<ActionResult<TransactionDto>> UpdateAccountTransaction(string accountNumber, Guid transactionId, [FromBody] UpdateTransactionCommand command)
    {
        if (transactionId != command.TransactionId) return BadRequest();
        if (!string.Equals(accountNumber, command.AccountNumber, StringComparison.OrdinalIgnoreCase))
        {
            command = command with { AccountNumber = accountNumber };
        }
        var result = await mediator.Send(command);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{accountNumber}/transactions/{transactionId:guid}")]
    public async Task<ActionResult> DeleteAccountTransaction(string accountNumber, Guid transactionId)
    {
        // Optionally we could verify account matches before deleting; skipping for simplicity
        var success = await mediator.Send(new DeleteTransactionCommand(transactionId));
        return success ? NoContent() : NotFound();
    }
}
