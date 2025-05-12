using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.People;
using MediatR;

namespace Application.Features.ServiceProviderManagers.Queries;

public sealed record GetServiceProviderManagerQuery(int UserId) : IRequest<Result<ServiceProviderManager>>;