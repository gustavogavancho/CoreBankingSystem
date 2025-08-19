using AutoMapper;
using CoreBankingSystem.Application.Abstractions;
using CoreBankingSystem.Application.Clients.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Application.Clients.Queries;

public record GetClientsQuery : IRequest<List<ClientDto>>;

public class GetClientsQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetClientsQuery, List<ClientDto>>
{
    public async Task<List<ClientDto>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
    {
        var clients = await context.Clients.AsNoTracking().ToListAsync(cancellationToken);
        return mapper.Map<List<ClientDto>>(clients);
    }
}
