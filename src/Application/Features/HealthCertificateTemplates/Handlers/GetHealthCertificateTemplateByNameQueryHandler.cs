using Application.Features.HealthCertificateTemplates.Queries;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
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
            return HealthCertificateTemplateErrors.NotFoundByName(request.HealthCertificateName);
        }

        return healthCertificateTemplate;
    }
}