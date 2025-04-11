using Domain.Abstractions;
using Domain.Models.Entities.People;
using MediatR;

namespace Application.Features.Clients.Queries;

public record GetClientByIdQuery(int Id) : IRequest<Result<Client>>;