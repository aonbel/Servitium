using Domain.Abstractions;
using Domain.Models.Entities.People;
using MediatR;

namespace Application.Features.Clients.Queries;

public record GetClientQuery(int Id) : IRequest<Result<Client>>;