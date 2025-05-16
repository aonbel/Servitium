using Application.Features.HealthCertificateTemplates.Queries;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
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
            return HealthCertificateTemplateErrors.NotFoundById(request.HealthCertificateTemplateId);
        }
        
        return healthCertificateTemplate;
    }
}