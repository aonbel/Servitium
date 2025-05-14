using Application.Features.Clients.Commands;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.People;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Clients.Handlers;

public sealed class CreateClientCommandHandler(
    IApplicationDbContext applicationDbContext) : IRequestHandler<CreateClientCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var personExists = await applicationDbContext.Persons.AnyAsync(
            p => p.Id == request.PersonId,
            cancellationToken);

        if (!personExists)
        {
            return PersonErrors.NotFoundById(request.PersonId);
        }

        var client = new Client
        {
            PersonId = request.PersonId,
            Birthday = request.Birthday,
            Gender = request.Gender
        };

        await applicationDbContext.Clients.AddAsync(client, cancellationToken);

        return await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}