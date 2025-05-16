using Application.Features.HealthCertificateTemplates.Queries;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.Services;
using Domain.Interfaces;
using Infrastructure.Interfaces;
using MediatR;

namespace Application.Features.HealthCertificateTemplates.Handlers;

public sealed class GetHealthCertificateTemplateByIdQueryHandler(IApplicationDbContext applicationDbContext) :
    IRequestHandler<GetHealthCertificateTemplateByIdQuery, Result<HealthCertificateTemplate>>
{
    public async Task<Result<HealthCertificateTemplate>> Handle(GetHealthCertificateTemplateByIdQuery request,
        CancellationToken cancellationToken)
    {
        var healthCertificateTemplate =
            await applicationDbContext.HealthCertificateTemplates.FindAsync([request.HealthCertificateTemplateId],
                cancellationToken);

        if (healthCertificateTemplate is null)
        {
            return new Error("HealthCertificateNotFound",
                $"Health certificate with given id {request.HealthCertificateTemplateId} does not exist.");
        }
        
        return healthCertificateTemplate;
    }
}