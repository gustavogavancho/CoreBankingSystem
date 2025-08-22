using AutoMapper;
using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Application.Clients.Models;
using MediatR;

namespace CoreBankingSystem.Application.Clients.Queries;

public record GetClientByIdentificationQuery(string Identification) : IRequest<ClientDto?>;

public class GetClientByIdentificationQueryHandler(IClientRepository repository, IMapper mapper)
    : IRequestHandler<GetClientByIdentificationQuery, ClientDto?>
{
    public async Task<ClientDto?> Handle(GetClientByIdentificationQuery request, CancellationToken cancellationToken)
    {
        var client = await repository.GetByIdentificationAsync(request.Identification);
        return client is null ? null : mapper.Map<ClientDto>(client);
    }
}
