using CoreBankingSystem.Application.Accounts.Models;
using CoreBankingSystem.Application.Accounts.Queries;
using CoreBankingSystem.Application.Accounts.Commands;
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

    [HttpGet("by-client/{clientId:guid}")]
    public async Task<ActionResult<List<AccountDto>>> GetByClient(Guid clientId)
    {
        var result = await mediator.Send(new GetAccountsByClientQuery(clientId));
        return Ok(result);
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
}
