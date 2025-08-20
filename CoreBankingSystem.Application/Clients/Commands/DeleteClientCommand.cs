using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Application.Common.Exceptions;
using MediatR;

namespace CoreBankingSystem.Application.Clients.Commands;

public record DeleteClientCommand(Guid Id) : IRequest<Unit>;

public class DeleteClientCommandHandler(IClientRepository repository)
    : IRequestHandler<DeleteClientCommand, Unit>
{
    public async Task<Unit> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) throw new NotFoundException("Client", request.Id);

        await repository.RemoveAsync(entity, cancellationToken);
        return Unit.Value;
    }
}
