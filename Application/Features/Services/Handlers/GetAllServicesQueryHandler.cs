using Application.Features.Services.Queries;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Models.Entities.Services;
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