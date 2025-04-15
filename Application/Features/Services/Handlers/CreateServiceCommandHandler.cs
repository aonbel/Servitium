using Application.Features.Services.Commands;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Models.Entities.Services;
using MediatR;

namespace Application.Features.Services.Handlers;

public sealed class CreateServiceCommandHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<CreateServiceCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        List<HealthCertificateTemplate> requirements = [];
        List<HealthCertificateTemplate> results = [];

        foreach (var requirementId in request.RequirementIds)
        {
            var requirement =
                await applicationDbContext.HealthСertificateTemplates.FindAsync([requirementId], cancellationToken);

            if (requirement is null)
            {
                return new Error("HealthCertificateTemplateNotFound",
                    $"Health certificate template with given id {requirementId} does not exist");
            }
            
            requirements.Add(requirement);
        }
        
        foreach (var resultId in request.ResultIds)
        {
            var result =
                await applicationDbContext.HealthСertificateTemplates.FindAsync([resultId], cancellationToken);

            if (result is null)
            {
                return new Error("HealthCertificateTemplateNotFound",
                    $"Health certificate template with given id {resultId} does not exist");
            }
            
            results.Add(result);
        }

        var service = new Service
        {
            Name = request.Name,
            ShortName = request.ShortName,
            Description = request.Description,
            Requirements = requirements,
            Result = results,
            PricePerHourForMaterials = request.PricePerHourForMaterials,
            PricePerHourForEquipment = request.PricePerHourForEquipment,
            Duration = request.Duration
        };
        
        applicationDbContext.Services.Add(service);
        
        return await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}