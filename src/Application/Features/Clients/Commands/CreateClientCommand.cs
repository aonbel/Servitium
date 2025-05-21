using Domain.Abstractions.Result;
using Domain.Entities.People;
using MediatR;

namespace Application.Features.Clients.Commands;

public sealed record CreateClientCommand(
    int PersonId,
    DateOnly Birthday,
    string Gender) : IRequest<Result<Client>>;