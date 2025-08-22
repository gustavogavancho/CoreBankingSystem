using System.Text;
using CoreBankingSystem.Application.Abstractions.Reports;
using CoreBankingSystem.Application.Reports.Models;

namespace CoreBankingSystem.Application.Reports.Services;

public class ReportTextFormatter : IReportTextFormatter
{
    public string BuildClientStatementText(ClientStatementReportDto dto)
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
