using Application.Features.HealthCertificateTemplates.Queries;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Models.Entities.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.HealthCertificateTemplates.Handlers;

public sealed class GetHealthCertificateTemplateByNameQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetHealthCertificateTemplateByNameQuery, Result<HealthCertificateTemplate>>
{
    public async Task<Result<HealthCertificateTemplate>> Handle(GetHealthCertificateTemplateByNameQuery request,
        CancellationToken cancellationToken)
    {
        var healthCertificateTemplate = await applicationDbContext.HealthСertificateTemplates
                .Where(template => template.Name == request.HealthCertificateName)
                .FirstOrDefaultAsync(cancellationToken);

        if (healthCertificateTemplate is null)
        {
            return new Error("HealthCertificateNotFound",
                $"Health certificate with given name {request.HealthCertificateName} does not exist.");
        }

        return healthCertificateTemplate;
    }
}