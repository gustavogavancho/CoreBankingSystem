using AutoMapper;
using CoreBankingSystem.Application.Abstractions;
using CoreBankingSystem.Application.Clients.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Application.Clients.Queries;

public record GetClientByClientIdQuery(Guid ClientId) : IRequest<ClientDto?>;

public class GetClientByClientIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetClientByClientIdQuery, ClientDto?>
{
    public async Task<ClientDto?> Handle(GetClientByClientIdQuery request, CancellationToken cancellationToken)
    {
        var client = await context.Clients.AsNoTracking()
            .FirstOrDefaultAsync(c => c.ClientId == request.ClientId, cancellationToken);
        return client is null ? null : mapper.Map<ClientDto>(client);
    }
}
