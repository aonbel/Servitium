using Application.Features.HealthСertificatates.Commands;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.Services;
using Domain.Interfaces;
using Infrastructure.Interfaces;
using MediatR;

namespace Application.Features.HealthСertificatates.Handlers;

public sealed class CreateHealthCertificateCommandHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<CreateHealthCertificateCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateHealthCertificateCommand request, CancellationToken cancellationToken)
    {
        var template = await applicationDbContext.HealthCertificateTemplates.FindAsync(
            [request.TemplateId],
            cancellationToken);

        if (template is null)
        {
            return HealthCertificateTemplateErrors.NotFoundById(request.TemplateId);
        }

        var client = await applicationDbContext.Clients.FindAsync([request.ClientId], cancellationToken);

        if (client is null)
        {
            return ClientErrors.NotFoundById(request.ClientId);
        }

        var healthCertificate = new HealthCertificate
        {
            Description = request.Description,
            ReceivingTime = DateOnly.FromDateTime(DateTime.UtcNow),
            TemplateId = request.TemplateId,
            ClientId = request.ClientId,
        };

        await applicationDbContext.HealthCertificates.AddAsync(healthCertificate, cancellationToken);

        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return healthCertificate.Id!;
    }
}