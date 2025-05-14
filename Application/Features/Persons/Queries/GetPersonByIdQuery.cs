using Domain.Abstractions.Result;
using Domain.Entities.People;
using MediatR;

namespace Application.Features.Persons.Queries;

public sealed record GetPersonByIdQuery(int Id) : IRequest<Result<Person>>;