using Application.Features.HealthСertificatates.Queries;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.Services;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.HealthСertificatates.Handlers;

public sealed class GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryHandler(
    IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQuery,
        Result<HealthCertificate>>
{
    public async Task<Result<HealthCertificate>> Handle(
        GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQuery request,
        CancellationToken cancellationToken)
    {
        var client = await applicationDbContext.Clients.FindAsync([request.ClientId], cancellationToken);

        if (client is null)
        {
            return ClientErrors.NotFoundById(request.ClientId);
        }

        var healthCertificate = await applicationDbContext.HealthCertificates
            .Where(c => c.ClientId == request.ClientId && c.TemplateId == request.HealthCertificateTemplateId)
            .OrderByDescending(c => c.ReceivingTime)
            .FirstOrDefaultAsync(cancellationToken);

        if (healthCertificate is null)
        {
            return HealthCertificateErrors.NotFoundByTemplateIdAmongHealthCertificatesOfClient(
                request.ClientId,
                request.HealthCertificateTemplateId);
        }

        return healthCertificate;
    }
}