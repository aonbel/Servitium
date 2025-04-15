using Application.Features.HealthСertificatates.Queries;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Models.Entities.Services;
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
            return new Error("ClientNotFound", "Client with given username does not exist");
        }

        var certificate = client.ServiceResults
            .Where(certificate => certificate.TemplateId == request.HealthCertificateTemplateId)
            .MaxBy(certificate => certificate.ReceivingTime);

        if (certificate is null)
        {
            return new Error("HealthCertificateNotFound",
                "Health certificate with given template id does not exist for given client");
        }
        
        return certificate;
    }
}