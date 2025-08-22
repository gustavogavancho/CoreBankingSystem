using CoreBankingSystem.Application.Reports.Models;

namespace CoreBankingSystem.Application.Abstractions.Reports;

public interface IReportTextFormatter
{
    string BuildClientStatementText(ClientStatementReportDto dto);
}
