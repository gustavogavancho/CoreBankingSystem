using AutoMapper;
using CoreBankingSystem.Application.Abstractions;
using CoreBankingSystem.Application.People.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Application.People.Commands;

public record UpdatePersonCommand(
    Guid Id,
    string Name,
    string Gender,
    int Age,
    string Identification,
    string PhoneNumber
) : IRequest<PersonDto?>;

public class UpdatePersonCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<UpdatePersonCommand, PersonDto?>
{
    public async Task<PersonDto?> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.People.FirstOrDefaultAsync(p => p.Id == request.Id);
        if (entity is null) return null;

        entity.Name = request.Name;
        entity.Gender = request.Gender;
        entity.Age = request.Age;
        entity.Identification = request.Identification;
        entity.PhoneNumber = request.PhoneNumber;

        await context.SaveChangesAsync();

        return mapper.Map<PersonDto>(entity);
    }
}
