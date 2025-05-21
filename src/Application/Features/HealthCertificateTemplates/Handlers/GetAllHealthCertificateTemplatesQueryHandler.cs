using Application.Features.HealthCertificateTemplates.Queries;
using Domain.Abstractions.Result;
using Domain.Entities.Services;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.HealthCertificateTemplates.Handlers;

public sealed class GetAllHealthCertificateTemplatesQueryHandler(IApplicationDbContext applicationDbContext) :
    IRequestHandler<GetAllHealthCertificateTemplatesQuery, Result<ICollection<HealthCertificateTemplate>>>
{
    public async Task<Result<ICollection<HealthCertificateTemplate>>> Handle(GetAllHealthCertificateTemplatesQuery request,
        CancellationToken cancellationToken)
    {
        var healthCertificateTemplates =
            await applicationDbContext.HealthCertificateTemplates.ToListAsync(cancellationToken);

        return healthCertificateTemplates;
    }
}