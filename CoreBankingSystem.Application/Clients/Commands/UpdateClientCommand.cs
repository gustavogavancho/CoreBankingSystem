using AutoMapper;
using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Application.Clients.Models;
using MediatR;

namespace CoreBankingSystem.Application.Clients.Commands;

public record UpdateClientCommand(
    Guid Id,
    string Name,
    string Gender,
    int Age,
    string Identification,
    string PhoneNumber,
    bool Status
) : IRequest<ClientDto?>;

public class UpdateClientCommandHandler(IClientRepository repository, IMapper mapper)
    : IRequestHandler<UpdateClientCommand, ClientDto?>
{
    public async Task<ClientDto?> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) return null;

        entity.Name = request.Name;
        entity.Gender = request.Gender;
        entity.Age = request.Age;
        entity.Identification = request.Identification;
        entity.PhoneNumber = request.PhoneNumber;
        entity.Status = request.Status;

        await repository.UpdateAsync(entity, cancellationToken);

        return mapper.Map<ClientDto>(entity);
    }
}
