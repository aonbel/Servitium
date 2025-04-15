using Domain.Abstractions;
using Domain.Models.Entities.People;
using MediatR;

namespace Application.Features.ServiceProviderManagers.Queries;

public sealed record GetServiceProviderManagerQuery(int UserId) : IRequest<Result<ServiceProviderManager>>;