using AutoMapper;
using CoreBankingSystem.Application.Abstractions;
using CoreBankingSystem.Application.Clients.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

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

public class UpdateClientCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<UpdateClientCommand, ClientDto?>
{
    public async Task<ClientDto?> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Clients.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (entity is null) return null;

        entity.Name = request.Name;
        entity.Gender = request.Gender;
        entity.Age = request.Age;
        entity.Identification = request.Identification;
        entity.PhoneNumber = request.PhoneNumber;
        entity.Status = request.Status;

        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<ClientDto>(entity);
    }
}
