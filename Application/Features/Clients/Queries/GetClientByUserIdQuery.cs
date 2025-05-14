using Domain.Abstractions.Result;
using Domain.Entities.People;
using MediatR;

namespace Application.Features.Clients.Queries;

public record GetClientByUserIdQuery(string UserId) : IRequest<Result<Client>>;