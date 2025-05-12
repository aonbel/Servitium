using Application.Features.Services.Queries;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.Services;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Services.Handlers;

public sealed class GetServiceByIdQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetServiceByIdQuery, Result<Service>>
{
    public async Task<Result<Service>> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
    {
        var service = await applicationDbContext.Services.FindAsync([request.ServiceId], cancellationToken);

        if (service is null)
        {
            return new Error("ServiceNotFound", $"Service with given id {request.ServiceId} does not exist");
        }
        
        return service;
    }
}