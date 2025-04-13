using Domain.Abstractions;
using Domain.Models.Entities.People;
using MediatR;

namespace Application.Features.Clients.Queries;

public sealed record GetClientByIdQuery(int Id) : IRequest<Result<Client>>;