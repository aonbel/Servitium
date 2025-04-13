using Domain.Abstractions;
using Domain.Models.Entities.Services;
using MediatR;

namespace Application.Features.ServiceProviders.Queries;

public sealed record GetServiceProviderQuery(int Id) : IRequest<Result<ServiceProvider>>;