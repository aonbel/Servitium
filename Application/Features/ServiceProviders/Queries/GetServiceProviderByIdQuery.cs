using Domain.Abstractions.Result;
using Domain.Entities.Services;
using MediatR;

namespace Application.Features.ServiceProviders.Queries;

public sealed record GetServiceProviderByIdQuery(int Id) : IRequest<Result<ServiceProvider>>;