using Domain.Abstractions.Result;
using Domain.Entities.People;
using MediatR;

namespace Application.Features.ServiceProviderManagers.Queries;

public record GetAllServiceProviderManagersQuery() : IRequest<Result<ICollection<ServiceProviderManager>>>;