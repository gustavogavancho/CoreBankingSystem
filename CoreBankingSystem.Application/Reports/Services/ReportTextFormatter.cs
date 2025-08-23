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

        // Tabla resumen por cuenta
        string[] headers = {
            "Cuenta", "Tipo", "Estado", "Saldo Inicial", "Creditos", "Debitos", "Saldo Final"
        };
        int[] widths = {
            14, 10, 8, 15, 12, 12, 15
        };

        sb.AppendLine(BuildRow(headers, widths));
        sb.AppendLine(new string('-', widths.Sum() + (headers.Length - 1) * 2));

        foreach (var a in dto.Accounts)
        {
            string[] row = {
                a.AccountNumber,
                a.AccountType,
                a.Status ? "Activo" : "Inactivo",
                a.StartingBalance.ToString("n2"),
                a.TotalCredits.ToString("n2"),
                a.TotalDebits.ToString("n2"),
                a.EndingBalance.ToString("n2")
            };
            sb.AppendLine(BuildRow(row, widths));

            // Movimientos por cuenta (si existen)
            if (a.Transactions is { Count: > 0 })
            {
                sb.AppendLine("  Movimientos:");
                string[] txHeaders = { "Fecha", "Tipo", "Monto", "Saldo" };
                int[] txWidths = { 12, 18, 14, 14 };
                sb.AppendLine("  " + BuildRow(txHeaders, txWidths));
                sb.AppendLine("  " + new string('-', txWidths.Sum() + (txHeaders.Length - 1) * 2));
                foreach (var t in a.Transactions.OrderBy(t => t.Date))
                {
                    string[] txRow = {
                        t.Date.ToString("yyyy-MM-dd"),
                        t.TransactionType,
                        t.Amount.ToString("n2"),
                        t.Balance.ToString("n2")
                    };
                    sb.AppendLine("  " + BuildRow(txRow, txWidths));
                }
            }

            sb.AppendLine(new string('-', widths.Sum() + (headers.Length - 1) * 2));
        }

        return sb.ToString();

        static string BuildRow(string[] cols, int[] w)
        {
            var parts = new List<string>(cols.Length);
            for (int i = 0; i < cols.Length; i++)
            {
                var val = cols[i] ?? string.Empty;
                // align numbers to the right for numeric columns (by index)
                bool isNumeric = i >= 3; // from Saldo Inicial onwards
                parts.Add(Align(val, w[i], isNumeric));
            }
            return string.Join("  ", parts);
        }

        static string Align(string value, int width, bool right)
        {
            if (value.Length > width)
                value = value[..width];
            return right ? value.PadLeft(width) : value.PadRight(width);
        }
    }
}
