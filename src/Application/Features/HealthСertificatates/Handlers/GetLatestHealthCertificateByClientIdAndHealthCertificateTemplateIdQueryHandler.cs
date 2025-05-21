using Application.Features.HealthСertificatates.Queries;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.Services;
using Domain.Interfaces;
using MediatR;

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

        var healthCertificate = applicationDbContext.HealthCertificates
            .Where(c => c.ClientId == request.ClientId && c.TemplateId == request.HealthCertificateTemplateId)
            .MaxBy(c => c.ReceivingTime);

        if (healthCertificate is null)
        {
            return HealthCertificateErrors.NotFoundByTemplateIdAmongHealthCertificatesOfClient(
                request.ClientId,
                request.HealthCertificateTemplateId);
        }

        return healthCertificate;
    }
}