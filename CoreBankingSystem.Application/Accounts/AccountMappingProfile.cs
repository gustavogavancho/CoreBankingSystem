using AutoMapper;
using CoreBankingSystem.Application.Accounts.Models;
using CoreBankingSystem.Domain.Entities;

namespace CoreBankingSystem.Application.Accounts;

public class AccountMappingProfile : Profile
{
    public AccountMappingProfile()
    {
        CreateMap<Account, AccountDto>()
            .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.ClientId));
    }
}
