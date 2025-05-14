using Application.Features.Persons.Commands;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.People;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Persons.Handlers;

public sealed class CreatePersonCommandHandler(
    UserManager<IdentityUser> userManager,
    IApplicationDbContext applicationDbContext)
    : IRequestHandler<CreatePersonCommand, Result<Person>>
{
    public async Task<Result<Person>> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        var userExists = userManager.Users.Any(u => u.Id == request.UserId);

        if (!userExists)
        {
            return UserErrors.NotFoundById(request.UserId);
        }
        
        var person = new Person
        {
            UserId = request.UserId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            Phone = request.Phone,
            Email = request.Email
        };
        
        await applicationDbContext.Persons.AddAsync(person, cancellationToken);
        
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        
        return person;
    }
}