using Domain.Abstractions.Result;
using Domain.Entities.Services;
using MediatR;

namespace Application.Features.ServiceProviders.Queries;

public record GetAllServiceProvidersByServiceAndDateTimeQuery(int ServiceId, DateTime DateTime) : IRequest<Result<ICollection<ServiceProvider>>>;