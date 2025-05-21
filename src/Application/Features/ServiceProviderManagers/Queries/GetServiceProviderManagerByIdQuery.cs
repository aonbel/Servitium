using Domain.Abstractions.Result;
using Domain.Entities.People;
using MediatR;

namespace Application.Features.ServiceProviderManagers.Queries;

public sealed record GetServiceProviderManagerByIdQuery(int Id) : IRequest<Result<ServiceProviderManager>>;