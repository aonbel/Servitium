using Application.Features.HealthCertificateTemplates.Commands;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Models.Entities.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.HealthCertificateTemplates.Handlers;

public sealed class CreateHealthCertificateTemplateCommandHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<CreateHealthCertificateTemplateCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateHealthCertificateTemplateCommand request,
        CancellationToken cancellationToken)
    {
        var healthCertificateTemplateWithGivenName =
            await applicationDbContext.HealthСertificateTemplates.Where(template => template.Name == request.Name)
                .FirstOrDefaultAsync(cancellationToken);

        if (healthCertificateTemplateWithGivenName is not null)
        {
            return new Error("HealthCertificateNameAlreadyExists",
                "Given health certificate name already exists");
        }

        var healthCertificateTemplate = new HealthCertificateTemplate
        {
            Name = request.Name,
            ActivePeriod = request.ActivePeriod
        };
        
        await applicationDbContext.HealthСertificateTemplates.AddAsync(healthCertificateTemplate, cancellationToken);
        
        return await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}