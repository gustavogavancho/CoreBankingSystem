using CoreBankingSystem.Application.Accounts.Models;
using CoreBankingSystem.Application.Accounts.Queries;
using CoreBankingSystem.Application.Accounts.Commands;
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
    public async Task<ActionResult<List<AccountDto>>> Get(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAccountsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{accountNumber}")]
    public async Task<ActionResult<AccountDto>> GetByNumber(string accountNumber, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAccountByNumberQuery(accountNumber), cancellationToken);
        return Ok(result);
    }

    // List transactions by account number
    [HttpGet("{accountNumber}/transactions")]
    public async Task<ActionResult<List<TransactionDto>>> GetTransactionsByAccount(string accountNumber, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetTransactionsByAccountNumberQuery(accountNumber), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<AccountDto>> Create(CreateAccountCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetByNumber), new { accountNumber = result.AccountNumber }, result);
    }

    [HttpPut("{accountNumber}")]
    public async Task<ActionResult<AccountDto>> Update(string accountNumber, UpdateAccountCommand command, CancellationToken cancellationToken)
    {
        if (!string.Equals(accountNumber, command.AccountNumber, StringComparison.OrdinalIgnoreCase)) return BadRequest();
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{accountNumber}")]
    public async Task<ActionResult> Delete(string accountNumber, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteAccountCommand(accountNumber), cancellationToken);
        return NoContent();
    }
}
