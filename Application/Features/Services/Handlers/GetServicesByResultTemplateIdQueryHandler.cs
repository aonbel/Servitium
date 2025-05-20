using Application.Features.Services.Queries;
using Domain.Abstractions.Result;
using Domain.Entities.Services;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Services.Handlers;

public class GetServicesByResultTemplateIdQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetServicesByResultTemplateIdQuery, Result<ICollection<Service>>>
{
    public async Task<Result<ICollection<Service>>> Handle(GetServicesByResultTemplateIdQuery request,
        CancellationToken cancellationToken)
    {
        var services = await applicationDbContext.Services
            .Where(s => s.ResultHealthCertificateTemplateIds.Contains(request.ResultTemplateId))
            .ToListAsync(cancellationToken);

        return services;
    }
}