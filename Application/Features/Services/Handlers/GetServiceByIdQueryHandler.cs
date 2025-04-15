using Application.Features.Services.Queries;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Models.Entities.Services;
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