using Domain.Abstractions.Result;
using Domain.Entities.People;
using MediatR;

namespace Application.Features.ServiceProviderManagers.Queries;

public record GetServiceProviderManagerByPersonIdQuery(int PersonId) : IRequest<Result<ServiceProviderManager>>;