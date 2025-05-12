using Application.Features.Services.Queries;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.Services;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Services.Handlers;

public sealed class GetAllServicesQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetAllServicesQuery, Result<ICollection<Service>>>
{
    public async Task<Result<ICollection<Service>>> Handle(GetAllServicesQuery request,
        CancellationToken cancellationToken)
    {
        var services = await applicationDbContext.Services.ToListAsync(cancellationToken);

        return services;
    }
}