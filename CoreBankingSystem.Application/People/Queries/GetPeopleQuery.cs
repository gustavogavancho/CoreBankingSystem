using AutoMapper;
using CoreBankingSystem.Application.Abstractions;
using CoreBankingSystem.Application.People.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Application.People.Queries;

public record GetPeopleQuery : IRequest<List<PersonDto>>;

public class GetPeopleQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetPeopleQuery, List<PersonDto>>
{
    public async Task<List<PersonDto>> Handle(GetPeopleQuery request, CancellationToken cancellationToken)
    {
        var people = await context.People.AsNoTracking().ToListAsync(cancellationToken);
        return mapper.Map<List<PersonDto>>(people);
    }
}
