using Application.Features.HealthCertificateTemplates.Commands;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.Services;
using Domain.Interfaces;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.HealthCertificateTemplates.Handlers;

public sealed class CreateHealthCertificateTemplateCommandHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<CreateHealthCertificateTemplateCommand, Result<HealthCertificateTemplate>>
{
    public async Task<Result<HealthCertificateTemplate>> Handle(CreateHealthCertificateTemplateCommand request,
        CancellationToken cancellationToken)
    {
        var healthCertificateTemplateWithGivenName =
            await applicationDbContext.HealthCertificateTemplates.Where(template => template.Name == request.Name)
                .FirstOrDefaultAsync(cancellationToken);

        if (healthCertificateTemplateWithGivenName is not null)
        {
            return HealthCertificateTemplateErrors.TemplateWithGivenNameAlreadyExists(request.Name);
        }

        var healthCertificateTemplate = new HealthCertificateTemplate
        {
            Name = request.Name,
            ActivePeriod = request.ActivePeriod
        };
        
        await applicationDbContext.HealthCertificateTemplates.AddAsync(healthCertificateTemplate, cancellationToken);
        
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        
        return healthCertificateTemplate;
    }
}