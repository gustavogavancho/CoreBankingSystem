using AutoMapper;
using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Application.Clients.Models;
using MediatR;

namespace CoreBankingSystem.Application.Clients.Queries;

public record GetClientsQuery : IRequest<List<ClientDto>>;

public class GetClientsQueryHandler(IClientRepository repository, IMapper mapper)
    : IRequestHandler<GetClientsQuery, List<ClientDto>>
{
    public async Task<List<ClientDto>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
    {
        var clients = await repository.GetAllAsync();
        return mapper.Map<List<ClientDto>>(clients);
    }
}
