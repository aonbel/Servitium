using Application.Features.Services.Commands;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.Services;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Services.Handlers;

public sealed class CreateServiceCommandHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<CreateServiceCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        foreach (var requirementId in request.RequirementIds)
        {
            var requirement =
                await applicationDbContext.HealthCertificateTemplates.FindAsync([requirementId], cancellationToken);

            if (requirement is null)
            {
                return new Error("HealthCertificateTemplateNotFound",
                    $"Health certificate template with given id {requirementId} does not exist");
            }
        }
        
        foreach (var resultId in request.ResultIds)
        {
            var result =
                await applicationDbContext.HealthCertificateTemplates.FindAsync([resultId], cancellationToken);

            if (result is null)
            {
                return new Error("HealthCertificateTemplateNotFound",
                    $"Health certificate template with given id {resultId} does not exist");
            }
        }

        var service = new Service
        {
            Name = request.Name,
            ShortName = request.ShortName,
            Description = request.Description,
            RequiredHealthCertificateTemplateIds = request.RequirementIds,
            ResultHealthCertificateTemplateIds = request.ResultIds,
            PricePerHourForMaterials = request.PricePerHourForMaterials,
            PricePerHourForEquipment = request.PricePerHourForEquipment,
            Duration = request.Duration
        };
        
        applicationDbContext.Services.Add(service);
        
        return await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}