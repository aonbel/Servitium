using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.Services;
using MediatR;

namespace Application.Features.ServiceProviders.Queries;

public sealed record GetServiceProviderQuery(int Id) : IRequest<Result<ServiceProvider>>;