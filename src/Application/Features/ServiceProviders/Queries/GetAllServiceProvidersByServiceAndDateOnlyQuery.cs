using Domain.Abstractions.Result;
using Domain.Entities.Services;
using MediatR;

namespace Application.Features.ServiceProviders.Queries;

public record GetAllServiceProvidersByServiceAndDateOnlyQuery(int ServiceId, DateOnly Date) : IRequest<Result<ICollection<ServiceProvider>>>;