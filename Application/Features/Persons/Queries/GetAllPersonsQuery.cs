using Domain.Abstractions.Result;
using Domain.Entities.People;
using MediatR;

namespace Application.Features.Persons.Queries;

public record GetAllPersonsQuery() : IRequest<Result<ICollection<Person>>>;