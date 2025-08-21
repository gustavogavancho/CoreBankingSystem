using AutoMapper;
using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Application.Clients.Models;
using CoreBankingSystem.Application.Common.Exceptions;
using MediatR;

namespace CoreBankingSystem.Application.Clients.Queries;

public record GetClientByIdQuery(Guid Id) : IRequest<ClientDto>;

public class GetClientByIdQueryHandler(IClientRepository repository, IMapper mapper)
    : IRequestHandler<GetClientByIdQuery, ClientDto>
{
    public async Task<ClientDto> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        var client = await repository.GetByIdAsync(request.Id);
        if (client is null) throw new NotFoundException(nameof(ClientDto), request.Id);
        return mapper.Map<ClientDto>(client);
    }
}
