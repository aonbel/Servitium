using Application.Features.HealthCertificateTemplates.Queries;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Models.Entities.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.HealthCertificateTemplates.Handlers;

public sealed class GetAllHealthCertificateTemplatesHandler(IApplicationDbContext applicationDbContext) :
    IRequestHandler<GetAllHealthCertificateTemplates, Result<ICollection<HealthCertificateTemplate>>>
{
    public async Task<Result<ICollection<HealthCertificateTemplate>>> Handle(GetAllHealthCertificateTemplates request,
        CancellationToken cancellationToken)
    {
        var healthCertificateTemplates =
            await applicationDbContext.HealthСertificateTemplates.ToListAsync(cancellationToken);

        return healthCertificateTemplates;
    }
}