using Application.Features.Persons.Queries;
using Domain.Abstractions.Result;
using Domain.Entities.People;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Persons.Handlers;

public class GetAllPersonsQueryHandler(IApplicationDbContext applicationDbContext) : IRequestHandler<GetAllPersonsQuery, Result<ICollection<Person>>>
{
    public async Task<Result<ICollection<Person>>> Handle(GetAllPersonsQuery request, CancellationToken cancellationToken)
    {
        var persons = await applicationDbContext.Persons.ToListAsync(cancellationToken);

        return persons;
    }
}