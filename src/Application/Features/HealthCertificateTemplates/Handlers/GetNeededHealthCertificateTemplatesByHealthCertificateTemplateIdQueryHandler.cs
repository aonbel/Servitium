using Application.Features.HealthCertificateTemplates.Queries;
using Application.Features.HealthCertificateTemplates.Responces;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.Services;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.HealthCertificateTemplates.Handlers;

public class GetNeededHealthCertificateTemplatesByHealthCertificateTemplateIdQueryHandler(
    IApplicationDbContext applicationDbContext) :
    IRequestHandler<GetNeededHealthCertificateTemplatesByHealthCertificateTemplateIdQuery,
        Result<GetNeededHealthCertificateTemplatesByHealthCertificateTemplateIdQueryResponse>>
{
    public async Task<Result<GetNeededHealthCertificateTemplatesByHealthCertificateTemplateIdQueryResponse>> Handle(
        GetNeededHealthCertificateTemplatesByHealthCertificateTemplateIdQuery request,
        CancellationToken cancellationToken)
    {
        var mainHealthCertificateTemplate =
            await applicationDbContext.HealthCertificateTemplates.FindAsync([request.HealthCertificateTemplateId],
                cancellationToken);

        if (mainHealthCertificateTemplate is null)
        {
            return HealthCertificateTemplateErrors.NotFoundById(request.HealthCertificateTemplateId);
        }

        Queue<int> healthCertificateTemplateIdQueue = new();

        healthCertificateTemplateIdQueue.Enqueue(request.HealthCertificateTemplateId);

        List<HealthCertificateTemplate> result = [];

        while (healthCertificateTemplateIdQueue.Count > 0)
        {
            var healthCertificateTemplateId = healthCertificateTemplateIdQueue.Dequeue();

            var healthCertificateTemplate =
                await applicationDbContext.HealthCertificateTemplates.FindAsync(
                    [healthCertificateTemplateId],
                    cancellationToken);

            if (healthCertificateTemplate is null)
            {
                return HealthCertificateTemplateErrors.NotFoundById(healthCertificateTemplateId);
            }
            
            result.Add(healthCertificateTemplate);

            var serviceProducingNeededTemplate = await applicationDbContext.Services
                .Where(s => s.ResultHealthCertificateTemplateIds.Contains(healthCertificateTemplateId))
                .FirstOrDefaultAsync(cancellationToken);

            if (serviceProducingNeededTemplate is null)
            {
                return new GetNeededHealthCertificateTemplatesByHealthCertificateTemplateIdQueryResponse(null);
            }

            var neededHealthCertificateTemplateIds =
                serviceProducingNeededTemplate.RequiredHealthCertificateTemplateIds;

            foreach (var templateId in neededHealthCertificateTemplateIds)
            {
                healthCertificateTemplateIdQueue.Enqueue(templateId);
            }
        }
        
        return new GetNeededHealthCertificateTemplatesByHealthCertificateTemplateIdQueryResponse(result);
    }
}