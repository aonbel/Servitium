using Domain.Abstractions.Result;
using Domain.Entities.Services;
using MediatR;

namespace Application.Features.ServiceProviders.Queries;

public record GetAllServiceProvidersQuery() : IRequest<Result<ICollection<ServiceProvider>>>;