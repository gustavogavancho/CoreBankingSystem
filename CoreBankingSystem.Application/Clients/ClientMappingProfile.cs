using AutoMapper;
using CoreBankingSystem.Application.Clients.Models;
using CoreBankingSystem.Domain.Entities;

namespace CoreBankingSystem.Application.Clients;

public class ClientMappingProfile : Profile
{
    public ClientMappingProfile()
    {
        CreateMap<Client, ClientDto>();
    }
}
