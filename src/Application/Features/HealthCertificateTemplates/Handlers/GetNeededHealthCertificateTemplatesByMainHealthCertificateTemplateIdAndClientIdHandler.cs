using Application.Features.HealthCertificateTemplates.Queries;
using Application.Features.HealthCertificateTemplates.Responces;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.HealthCertificateTemplates.Handlers;

public class
    GetNeededHealthCertificateTemplatesByMainHealthCertificateTemplateIdAndClientIdHandler(
        IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetNeededHealthCertificateTemplatesByMainHealthCertificateTemplateIdAndClientId,
        Result<GetNeededHealthCertificateTemplatesByMainHealthCertificateTemplateIdAndClientIdResponse>>
{
    public async Task<Result<GetNeededHealthCertificateTemplatesByMainHealthCertificateTemplateIdAndClientIdResponse>>
        Handle(GetNeededHealthCertificateTemplatesByMainHealthCertificateTemplateIdAndClientId request,
            CancellationToken cancellationToken)
    {
        var client = await applicationDbContext.Clients.FindAsync([request.ClientId], cancellationToken);

        if (client is null)
        {
            return ClientErrors.NotFoundById(request.ClientId);
        }

        var mainHealthCertificateTemplate =
            await applicationDbContext.HealthCertificateTemplates.FindAsync([request.HealthCertificateTemplateId],
                cancellationToken);

        if (mainHealthCertificateTemplate is null)
        {
            return HealthCertificateTemplateErrors.NotFoundById(request.HealthCertificateTemplateId);
        }

        Queue<int> healthCertificateTemplateIdQueue = new();

        healthCertificateTemplateIdQueue.Enqueue(request.HealthCertificateTemplateId);

        List<(TypeOfResult, int)> result = [];

        while (healthCertificateTemplateIdQueue.Count > 0)
        {
            var healthCertificateTemplateId = healthCertificateTemplateIdQueue.Dequeue();

            var neededHealthCertificateTemplateIdsResult =
                await GetNeededHealthCertificateTemplatesIds(healthCertificateTemplateId, cancellationToken);

            if (neededHealthCertificateTemplateIdsResult.IsError)
            {
                return neededHealthCertificateTemplateIdsResult.Error;
            }

            var neededHealthCertificateTemplateIds = neededHealthCertificateTemplateIdsResult.Value;

            foreach (var templateId in neededHealthCertificateTemplateIds)
            {
                var certificate = await applicationDbContext.HealthCertificates
                    .FirstOrDefaultAsync(c => c.TemplateId == templateId, cancellationToken);

                if (certificate is not null)
                {
                    result.Add((TypeOfResult.HealthCertificateId, certificate.Id ?? 0));

                    continue;
                }

                var servicesProducingTemplateIds = await applicationDbContext.Services
                    .Where(s => s.ResultHealthCertificateTemplateIds.Contains(templateId))
                    .Select(s => s.Id)
                    .ToListAsync(cancellationToken);

                var appointment = await applicationDbContext.Appointments
                    .FirstOrDefaultAsync(
                        a => a.ClientId == request.ClientId && servicesProducingTemplateIds.Contains(a.ServiceId),
                        cancellationToken);

                if (appointment is not null)
                {
                    result.Add((TypeOfResult.AppointmentId, appointment.Id ?? 0));

                    continue;
                }

                result.Add((TypeOfResult.HealthCertificateTemplateId, templateId));

                healthCertificateTemplateIdQueue.Enqueue(templateId);
            }   
        }

        return new GetNeededHealthCertificateTemplatesByMainHealthCertificateTemplateIdAndClientIdResponse(result);
    }

    private async Task<Result<ICollection<int>>> GetNeededHealthCertificateTemplatesIds(int healthCertificateTemplateId,
        CancellationToken cancellationToken)
    {
        var serviceProducingNeededTemplate = await applicationDbContext.Services
            .Where(s => s.ResultHealthCertificateTemplateIds.Contains(healthCertificateTemplateId))
            .FirstOrDefaultAsync(cancellationToken);

        if (serviceProducingNeededTemplate is null)
        {
            return ServiceErrors.NotFoundByResultHealthCertificateTemplateId(healthCertificateTemplateId);
        }

        return serviceProducingNeededTemplate.RequiredHealthCertificateTemplateIds.ToList();
    }
}