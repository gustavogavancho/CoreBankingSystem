namespace CoreBankingSystem.Application.Abstractions.Reports;

public interface IPdfGenerator
{
    byte[] GenerateFromText(string text);
}
