using Application.Features.HealthСertificatates.Commands;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.Services;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.HealthСertificatates.Handlers;

public sealed class CreateHealthCertificateCommandHandler(IApplicationDbContext applicationDbContext) : IRequestHandler<CreateHealthCertificateCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateHealthCertificateCommand request, CancellationToken cancellationToken)
    {
        var template = await applicationDbContext.HealthCertificateTemplates.FindAsync(
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
        
        await applicationDbContext.HealthCertificates.AddAsync(healthCertificate, cancellationToken);
        
        client.ServiceResults.Add(healthCertificate);
        
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        
        return healthCertificate.Id!;
    }
}