using Application.Features.HealthCertificateTemplates.Queries;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.Services;
using Domain.Interfaces;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.HealthCertificateTemplates.Handlers;

public sealed class GetAllHealthCertificateTemplatesHandler(IApplicationDbContext applicationDbContext) :
    IRequestHandler<GetAllHealthCertificateTemplates, Result<ICollection<HealthCertificateTemplate>>>
{
    public async Task<Result<ICollection<HealthCertificateTemplate>>> Handle(GetAllHealthCertificateTemplates request,
        CancellationToken cancellationToken)
    {
        var healthCertificateTemplates =
            await applicationDbContext.HealthCertificateTemplates.ToListAsync(cancellationToken);

        return healthCertificateTemplates;
    }
}