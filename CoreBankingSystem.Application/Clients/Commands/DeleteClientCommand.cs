using CoreBankingSystem.Application.Abstractions.Repositories;
using MediatR;

namespace CoreBankingSystem.Application.Clients.Commands;

public record DeleteClientCommand(Guid Id) : IRequest<bool>;

public class DeleteClientCommandHandler(IClientRepository repository)
    : IRequestHandler<DeleteClientCommand, bool>
{
    public async Task<bool> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) return false;

        await repository.RemoveAsync(entity, cancellationToken);
        return true;
    }
}
