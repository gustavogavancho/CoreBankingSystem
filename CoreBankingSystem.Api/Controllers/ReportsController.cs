using CoreBankingSystem.Application.Abstractions.Reports;
using CoreBankingSystem.Application.Reports.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CoreBankingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController(IMediator mediator, IReportTextFormatter formatter, IPdfGenerator pdfGenerator) : ControllerBase
{
    // JSON endpoint - keep only /report/json
    [HttpGet("/report/json")] // /report/json?Cliente=...&rangoFechas=...
    public async Task<IActionResult> GetClientStatementJson(
        [FromQuery] Guid? clientId,
        [FromQuery(Name = "Cliente")] Guid? cliente,
        [FromQuery] DateTime? start,
        [FromQuery] DateTime? end,
        [FromQuery(Name = "rangoFechas")] string? rangoFechas,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new GetClientStatementReportFromParamsQuery(clientId, cliente, start, end, rangoFechas), cancellationToken);
        return Ok(dto);
    }

    // PDF endpoint (returns base64) - keep only /report/pdf
    [HttpGet("/report/pdf")] // /report/pdf?Cliente=...&rangoFechas=...
    public async Task<IActionResult> GetClientStatementPdf(
        [FromQuery] Guid? clientId,
        [FromQuery(Name = "Cliente")] Guid? cliente,
        [FromQuery] DateTime? start,
        [FromQuery] DateTime? end,
        [FromQuery(Name = "rangoFechas")] string? rangoFechas,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new GetClientStatementReportFromParamsQuery(clientId, cliente, start, end, rangoFechas), cancellationToken);
        var content = formatter.BuildClientStatementText(dto);
        var pdfBytes = pdfGenerator.GenerateFromText(content);
        var base64 = Convert.ToBase64String(pdfBytes);
        return Ok(new { base64 });
    }
}
