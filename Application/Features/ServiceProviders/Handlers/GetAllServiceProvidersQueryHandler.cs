using Application.Features.ServiceProviders.Queries;
using Domain.Abstractions.Result;
using Domain.Entities.Services;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ServiceProviders.Handlers;

public class GetAllServiceProvidersQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetAllServiceProvidersQuery, Result<ICollection<ServiceProvider>>>
{
    public async Task<Result<ICollection<ServiceProvider>>> Handle(GetAllServiceProvidersQuery request,
        CancellationToken cancellationToken)
    {
        var serviceProviders =
            await applicationDbContext.ServiceProviders.ToListAsync(cancellationToken: cancellationToken);
        
        return serviceProviders;
    }
}