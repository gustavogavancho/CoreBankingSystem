using AutoMapper;
using CoreBankingSystem.Application.Transactions.Models;
using CoreBankingSystem.Domain.Entities;

namespace CoreBankingSystem.Application.Transactions;

public class TransactionMappingProfile : Profile
{
    public TransactionMappingProfile()
    {
        CreateMap<Transaction, TransactionDto>().ReverseMap();
    }
}
