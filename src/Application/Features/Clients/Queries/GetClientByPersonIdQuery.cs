using Domain.Abstractions.Result;
using Domain.Entities.People;
using MediatR;

namespace Application.Features.Clients.Queries;

public record GetClientByPersonIdQuery(int PersonId) : IRequest<Result<Client>>;