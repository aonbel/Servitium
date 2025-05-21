using Application.Features.HealthСertificatates.Queries;
using Domain.Abstractions.Result;
using Domain.Entities.Services;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.HealthСertificatates.Handlers;

public class GetHealthCertificatesByClientIdQueryHandler(IApplicationDbContext applicationDbContext) :
    IRequestHandler<GetHealthCertificatesByClientIdQuery, Result<ICollection<HealthCertificate>>>
{
    public async Task<Result<ICollection<HealthCertificate>>> Handle(GetHealthCertificatesByClientIdQuery request,
        CancellationToken cancellationToken)
    {
        var healthCertificates = await applicationDbContext.HealthCertificates
            .Where(c => c.ClientId == request.ClientId)
            .ToListAsync(cancellationToken);

        return healthCertificates;
    }
}