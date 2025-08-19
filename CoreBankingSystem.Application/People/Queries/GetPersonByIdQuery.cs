using AutoMapper;
using CoreBankingSystem.Application.Abstractions;
using CoreBankingSystem.Application.People.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Application.People.Queries;

public record GetPersonByIdQuery(Guid Id) : IRequest<PersonDto?>;

public class GetPersonByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetPersonByIdQuery, PersonDto?>
{
    public async Task<PersonDto?> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
    {
        var person = await context.People.AsNoTracking().FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        return person is null ? null : mapper.Map<PersonDto>(person);
    }
}
