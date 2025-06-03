using Application.Features.HealthСertificatates.Commands;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.HealthСertificatates.Handlers;

public class UpdateHealthCertificateStatusCommandHandler(IApplicationDbContext applicationDbContext) :
    IRequestHandler<UpdateHealthCertificateStatusCommand, Result>
{
    public async Task<Result> Handle(UpdateHealthCertificateStatusCommand request, CancellationToken cancellationToken)
    {
        var healthCertificate =
            await applicationDbContext.HealthCertificates.FindAsync([request.CertificateId], cancellationToken);

        if (healthCertificate is null)
        {
            return HealthCertificateErrors.NotFoundById(request.CertificateId);
        }
        
        healthCertificate.Description = request.Description;
        healthCertificate.ReceivingTime = DateOnly.FromDateTime(DateTime.UtcNow);
        
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}