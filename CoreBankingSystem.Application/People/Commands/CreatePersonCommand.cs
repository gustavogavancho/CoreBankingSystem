using AutoMapper;
using CoreBankingSystem.Application.Abstractions;
using CoreBankingSystem.Application.People.Models;
using CoreBankingSystem.Domain.Entities;
using MediatR;

namespace CoreBankingSystem.Application.People.Commands;

public record CreatePersonCommand(
    string Name,
    string Gender,
    int Age,
    string Identification,
    string PhoneNumber
) : IRequest<PersonDto>;

public class CreatePersonCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<CreatePersonCommand, PersonDto>
{
    public async Task<PersonDto> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        var entity = new Person
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Gender = request.Gender,
            Age = request.Age,
            Identification = request.Identification,
            PhoneNumber = request.PhoneNumber
        };

        context.People.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<PersonDto>(entity);
    }
}
