using Application.Features.HealthСertificatates.Queries;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.Services;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.HealthСertificatates.Handlers;

public class GetHealthCertificateByIdQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetHealthCertificateByIdQuery, Result<HealthCertificate>>
{
    public async Task<Result<HealthCertificate>> Handle(GetHealthCertificateByIdQuery request,
        CancellationToken cancellationToken)
    {
        var certificate =
            await applicationDbContext.HealthCertificates.SingleOrDefaultAsync(c => c.Id == request.Id,
                cancellationToken);

        if (certificate is null)
        {
            return HealthCertificateErrors.NotFoundById(request.Id);
        }

        return certificate;
    }
}