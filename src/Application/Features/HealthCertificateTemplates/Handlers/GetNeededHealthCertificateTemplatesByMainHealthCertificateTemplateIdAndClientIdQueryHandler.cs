using Application.Features.HealthCertificateTemplates.Queries;
using Application.Features.HealthCertificateTemplates.Responces;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.HealthCertificateTemplates.Handlers;

public class
    GetNeededHealthCertificateTemplatesByMainHealthCertificateTemplateIdAndClientIdQueryHandler(
        IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetNeededHealthCertificateTemplatesByMainHealthCertificateTemplateIdAndClientIdQuery,
        Result<GetNeededHealthCertificateTemplatesByMainHealthCertificateTemplateIdAndClientIdQueryResponse>>
{
    public async Task<
            Result<GetNeededHealthCertificateTemplatesByMainHealthCertificateTemplateIdAndClientIdQueryResponse>>
        Handle(GetNeededHealthCertificateTemplatesByMainHealthCertificateTemplateIdAndClientIdQuery request,
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

        List<Requirement> result = [];

        while (healthCertificateTemplateIdQueue.Count > 0)
        {
            var healthCertificateTemplateId = healthCertificateTemplateIdQueue.Dequeue();

            var checkTemplateResult = await CheckTemplate(
                healthCertificateTemplateId, 
                client.Id ?? 0, 
                cancellationToken);

            if (checkTemplateResult.IsError)
            {
                return checkTemplateResult.Error;
            }
            
            var requirement = checkTemplateResult.Value;
            
            result.Add(requirement);

            var neededHealthCertificateTemplateIdsResult =
                await GetNeededHealthCertificateTemplatesIds(healthCertificateTemplateId, cancellationToken);

            if (neededHealthCertificateTemplateIdsResult.IsError)
            {
                return neededHealthCertificateTemplateIdsResult.Error;
            }

            var neededHealthCertificateTemplateIds = neededHealthCertificateTemplateIdsResult.Value;

            foreach (var templateId in neededHealthCertificateTemplateIds)
            {
                healthCertificateTemplateIdQueue.Enqueue(templateId);
            }
        }

        return new GetNeededHealthCertificateTemplatesByMainHealthCertificateTemplateIdAndClientIdQueryResponse(result);
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

    private async Task<Result<Requirement>> CheckTemplate(
        int healthCertificateTemplateId,
        int clientId,
        CancellationToken cancellationToken)
    {
        var template = await applicationDbContext.HealthCertificateTemplates.FindAsync(
            [healthCertificateTemplateId],
            cancellationToken);

        if (template is null)
        {
            return HealthCertificateTemplateErrors.NotFoundById(healthCertificateTemplateId);
        }

        var certificate = await applicationDbContext.HealthCertificates
            .Where(c => c.TemplateId == healthCertificateTemplateId)
            .OrderByDescending(c => c.ReceivingTime)
            .FirstOrDefaultAsync(cancellationToken);

        if (certificate is not null &&
            certificate.ReceivingTime.AddDays(template.ActivePeriod.Days) >=
            DateOnly.FromDateTime(DateTime.Today))
        {
            return new Requirement(TypeOfRequirement.HealthCertificateId, certificate.Id ?? 0);
        }

        var servicesProducingTemplateIds = await applicationDbContext.Services
            .Where(s => s.ResultHealthCertificateTemplateIds.Contains(healthCertificateTemplateId))
            .Select(s => s.Id)
            .ToListAsync(cancellationToken);

        var appointment = await applicationDbContext.Appointments
            .FirstOrDefaultAsync(
                a => a.ClientId == clientId && servicesProducingTemplateIds.Contains(a.ServiceId),
                cancellationToken);

        if (appointment is not null)
        {
            return new Requirement(TypeOfRequirement.AppointmentId, appointment.Id ?? 0);
        }

        return new Requirement(TypeOfRequirement.HealthCertificateTemplateId, healthCertificateTemplateId);
    }
}