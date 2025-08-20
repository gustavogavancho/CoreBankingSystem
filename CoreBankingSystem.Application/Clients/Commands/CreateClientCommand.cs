using AutoMapper;
using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Application.Clients.Models;
using CoreBankingSystem.Domain.Entities;
using MediatR;

namespace CoreBankingSystem.Application.Clients.Commands;

public record CreateClientCommand(
    string Name,
    string Gender,
    int Age,
    string Identification,
    string PhoneNumber,
    string Password,
    bool Status
) : IRequest<ClientDto>;

public class CreateClientCommandHandler(IClientRepository repository, IMapper mapper)
    : IRequestHandler<CreateClientCommand, ClientDto>
{
    public async Task<ClientDto> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var entity = new Client
        {
            Id = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
            Name = request.Name,
            Gender = request.Gender,
            Age = request.Age,
            Identification = request.Identification,
            PhoneNumber = request.PhoneNumber,
            Password = request.Password,
            Status = request.Status
        };

        await repository.AddAsync(entity, cancellationToken);
        return mapper.Map<ClientDto>(entity);
    }
}
