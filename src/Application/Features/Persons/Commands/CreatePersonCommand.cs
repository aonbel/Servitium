using Domain.Abstractions.Result;
using Domain.Entities.People;
using MediatR;

namespace Application.Features.Persons.Commands;

public sealed record CreatePersonCommand(
    string UserId,
    string FirstName,
    string LastName,
    string MiddleName,
    string Phone,
    string Email) : IRequest<Result<Person>>;