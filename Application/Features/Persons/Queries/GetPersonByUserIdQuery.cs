using Domain.Abstractions.Result;
using Domain.Entities.People;
using MediatR;

namespace Application.Features.Persons.Queries;

public record GetPersonByUserIdQuery(string UserId) : IRequest<Result<Person>>;