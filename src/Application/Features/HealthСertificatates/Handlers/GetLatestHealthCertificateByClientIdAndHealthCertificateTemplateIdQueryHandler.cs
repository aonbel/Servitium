using Application.Features.HealthСertificatates.Queries;
using Application.Features.HealthСertificatates.Responses;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.HealthСertificatates.Handlers;

public sealed class GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryHandler(
    IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQuery,
        Result<GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryResponse>>
{
    public async Task<Result<GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryResponse>> Handle(
        GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQuery request,
        CancellationToken cancellationToken)
    {
        var client = await applicationDbContext.Clients.FindAsync([request.ClientId], cancellationToken);

        if (client is null)
        {
            return ClientErrors.NotFoundById(request.ClientId);
        }

        var template =
            await applicationDbContext.HealthCertificateTemplates.FindAsync(
                [request.HealthCertificateTemplateId],
                cancellationToken);

        if (template is null)
        {
            return HealthCertificateTemplateErrors.NotFoundById(request.HealthCertificateTemplateId);
        }

        var healthCertificate = applicationDbContext.HealthCertificates
            .Where(c => c.ClientId == request.ClientId && c.TemplateId == request.HealthCertificateTemplateId)
            .OrderByDescending(c => c.ReceivingTime)
            .FirstOrDefault();

        return new GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryResponse(healthCertificate);
    }
}