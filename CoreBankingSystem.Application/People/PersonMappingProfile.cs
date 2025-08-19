using AutoMapper;
using CoreBankingSystem.Application.People.Models;
using CoreBankingSystem.Domain.Entities;

namespace CoreBankingSystem.Application.People;

public class PersonMappingProfile : Profile
{
    public PersonMappingProfile()
    {
        CreateMap<Person, PersonDto>().ReverseMap();
    }
}
