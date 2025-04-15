using Application.Features.HealthСertificatates.Commands;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Models.Entities.Services;
using MediatR;

namespace Application.Features.HealthСertificatates.Handlers;

public sealed class CreateHealthCertificateCommandHandler(IApplicationDbContext applicationDbContext) : IRequestHandler<CreateHealthCertificateCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateHealthCertificateCommand request, CancellationToken cancellationToken)
    {
        var template = await applicationDbContext.HealthСertificateTemplates.FindAsync(
            [ request.TemplateId ], 
            cancellationToken);

        if (template is null)
        {
            return new Error("HealthCertificateNotFound", "Health certificate with given username does not exist");
        }
        
        var client = await applicationDbContext.Clients.FindAsync([request.ClientId], cancellationToken);

        if (client is null)
        {
            return new Error("ClientNotFound", "Client with given username does not exist");
        }

        var healthCertificate = new HealthCertificate
        {
            Name = template.Name,
            Description = request.Description,
            ReceivingTime = DateOnly.FromDateTime(DateTime.Now),
            ActivePeriod = template.ActivePeriod,
            TemplateId = request.TemplateId,
        };
        
        await applicationDbContext.HealthСertificates.AddAsync(healthCertificate, cancellationToken);
        
        client.ServiceResults.Add(healthCertificate);
        
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        
        return healthCertificate.Id!;
    }
}