using Application.Features.HealthCertificateTemplates.Queries;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.Services;
using Domain.Interfaces;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.HealthCertificateTemplates.Handlers;

public sealed class GetHealthCertificateTemplateByNameQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetHealthCertificateTemplateByNameQuery, Result<HealthCertificateTemplate>>
{
    public async Task<Result<HealthCertificateTemplate>> Handle(GetHealthCertificateTemplateByNameQuery request,
        CancellationToken cancellationToken)
    {
        var healthCertificateTemplate = await applicationDbContext.HealthCertificateTemplates
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