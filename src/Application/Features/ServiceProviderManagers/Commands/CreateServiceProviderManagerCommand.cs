using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.People;
using MediatR;

namespace Application.Features.ServiceProviderManagers.Commands;

public sealed record CreateServiceProviderManagerCommand(
    int PersonId,
    int ServiceProviderId) : IRequest<Result<ServiceProviderManager>>;