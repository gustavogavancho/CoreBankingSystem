using System.Text;
using CoreBankingSystem.Application.Reports.Models;
using CoreBankingSystem.Application.Reports.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CoreBankingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController(IMediator mediator) : ControllerBase
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
        var content = ReportTextFormatter.BuildPlainText(dto);
        var pdfBytes = SimplePdfGenerator.Generate(content);
        var base64 = Convert.ToBase64String(pdfBytes);
        return Ok(new { base64 });
    }
}

internal static class ReportTextFormatter
{
    public static string BuildPlainText(ClientStatementReportDto dto)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Estado de cuenta - Cliente: {dto.Name} ({dto.ClientId})");
        sb.AppendLine($"Rango: {dto.StartDate:yyyy-MM-dd} a {dto.EndDate:yyyy-MM-dd}");
        sb.AppendLine();
        foreach (var a in dto.Accounts)
        {
            sb.AppendLine($"Cuenta: {a.AccountNumber} - Tipo: {a.AccountType} - Estado: {(a.Status ? "Activo" : "Inactivo")}");
            sb.AppendLine($"Saldo Inicial: {a.StartingBalance:n2}");
            sb.AppendLine($"Créditos: {a.TotalCredits:n2}  Débitos: {a.TotalDebits:n2}");
            sb.AppendLine($"Saldo Final: {a.EndingBalance:n2}");
            if (a.Transactions is { Count: > 0 })
            {
                sb.AppendLine("Transacciones:");
                foreach (var t in a.Transactions.OrderBy(t => t.Date))
                {
                    sb.AppendLine($" - {t.Date:yyyy-MM-dd}: {t.TransactionType} {(t.Amount >= 0 ? "+" : "-")}{Math.Abs(t.Amount):n2} | Saldo: {t.Balance:n2}");
                }
            }
            sb.AppendLine(new string('-', 60));
        }
        return sb.ToString();
    }
}

internal static class SimplePdfGenerator
{
    // Minimal PDF builder producing a single-page PDF with the given text.
    public static byte[] Generate(string text)
    {
        string sanitized = text.Replace("(", "[").Replace(")", "]");
        string contentStream = $"BT /F1 10 Tf 72 750 Td ({sanitized.Replace("\\n", ") Tj T* (")}) Tj ET";

        var objects = new List<string>();
        objects.Add("1 0 obj<< /Type /Catalog /Pages 2 0 R >>endobj");
        objects.Add("2 0 obj<< /Type /Pages /Kids [3 0 R] /Count 1 >>endobj");
        objects.Add("3 0 obj<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 5 0 R >> >> /Contents 4 0 R >>endobj");
        objects.Add($"4 0 obj<< /Length {contentStream.Length} >>stream\n{contentStream}\nendstream endobj");
        objects.Add("5 0 obj<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>endobj");

        var sb = new StringBuilder();
        sb.Append("%PDF-1.4\n");
        var offsets = new List<int>();
        sb.Append("%\u00e2\u00e3\u00cf\u00d3\n");
        foreach (var obj in objects.Select((val, idx) => new { val, idx }))
        {
            offsets.Add(sb.Length);
            sb.Append(obj.val);
            sb.Append("\n");
        }
        int xrefPos = sb.Length;
        sb.Append($"xref\n0 {objects.Count + 1}\n");
        sb.Append("0000000000 65535 f \n");
        foreach (var off in offsets)
        {
            sb.Append(off.ToString("D10"));
            sb.Append(" 00000 n \n");
        }
        sb.Append("trailer<< /Size ");
        sb.Append(objects.Count + 1);
        sb.Append(" /Root 1 0 R >>\nstartxref\n");
        sb.Append(xrefPos);
        sb.Append("\n%%EOF");
        return Encoding.ASCII.GetBytes(sb.ToString());
    }
}
