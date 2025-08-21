using CoreBankingSystem.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Application.People.Commands;

public record DeletePersonCommand(Guid Id) : IRequest<bool>;

public class DeletePersonCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeletePersonCommand, bool>
{
    public async Task<bool> Handle(DeletePersonCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.People.FirstOrDefaultAsync(p => p.Id == request.Id);
        if (entity is null) return false;

        context.People.Remove(entity);
        await context.SaveChangesAsync();

        return true;
    }
}
