using AutoMapper;
using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Application.Clients.Models;
using MediatR;

namespace CoreBankingSystem.Application.Clients.Queries;

public record GetClientByClientIdQuery(Guid ClientId) : IRequest<ClientDto?>;

public class GetClientByClientIdQueryHandler(IClientRepository repository, IMapper mapper)
    : IRequestHandler<GetClientByClientIdQuery, ClientDto?>
{
    public async Task<ClientDto?> Handle(GetClientByClientIdQuery request, CancellationToken cancellationToken)
    {
        var client = await repository.GetByClientIdAsync(request.ClientId);
        return client is null ? null : mapper.Map<ClientDto>(client);
    }
}
