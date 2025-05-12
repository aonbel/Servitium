using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.People;
using MediatR;

namespace Application.Features.Clients.Queries;

public sealed record GetClientByIdQuery(int ClientId) : IRequest<Result<Client>>;