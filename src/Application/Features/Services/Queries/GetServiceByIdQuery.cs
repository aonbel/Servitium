using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.Services;
using MediatR;

namespace Application.Features.Services.Queries;

public sealed record GetServiceByIdQuery(int ServiceId) : IRequest<Result<Service>>;