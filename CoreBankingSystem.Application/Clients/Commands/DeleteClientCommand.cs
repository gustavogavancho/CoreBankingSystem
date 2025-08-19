using AutoMapper;
using CoreBankingSystem.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Application.Clients.Commands;

public record DeleteClientCommand(Guid Id) : IRequest<bool>;

public class DeleteClientCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteClientCommand, bool>
{
    public async Task<bool> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Clients.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (entity is null) return false;

        context.Clients.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
