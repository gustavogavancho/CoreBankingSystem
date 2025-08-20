using AutoMapper;
using CoreBankingSystem.Application.Transactions.Models;
using CoreBankingSystem.Domain.Entities;

namespace CoreBankingSystem.Application.Transactions;

public class TransactionMappingProfile : Profile
{
    public TransactionMappingProfile()
    {
        CreateMap<Transaction, TransactionDto>()
            .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.AccountNumber))
            .ReverseMap()
            .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.AccountNumber));
    }
}
