using Application.Features.HealthСertificatates.Queries;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.Services;
using Domain.Interfaces;
using Infrastructure.Interfaces;
using MediatR;

namespace Application.Features.HealthСertificatates.Handlers;

public sealed class GetLatestHealthCertificateQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetLatestHealthCertificateQuery, Result<HealthCertificate>>
{
    public async Task<Result<HealthCertificate>> Handle(GetLatestHealthCertificateQuery request,
        CancellationToken cancellationToken)
    {
        var client = await applicationDbContext.Clients.FindAsync([request.ClientId], cancellationToken);

        if (client is null)
        {
            return ClientErrors.NotFoundById(request.ClientId);
        }

        foreach (var serviceResultsId in client.ServiceResultsIds)
        {
            var healthCertificate =
                (await applicationDbContext.HealthCertificates.FindAsync([serviceResultsId], cancellationToken))!;

            if (healthCertificate.TemplateId == request.HealthCertificateTemplateId)
            {
                return healthCertificate;
            }
        }

        return HealthCertificateErrors.NotFoundByTemplateIdAmongHealthCertificatesOfClient(
            request.ClientId,
            request.HealthCertificateTemplateId);
    }
}