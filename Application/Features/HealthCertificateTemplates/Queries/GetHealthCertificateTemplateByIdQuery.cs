using Domain.Abstractions;
using Domain.Models.Entities.Services;
using MediatR;

namespace Application.Features.HealthCertificateTemplates.Queries;

public record GetHealthCertificateTemplateByIdQuery(int HealthCertificateTemplateId)
    : IRequest<Result<HealthCertificateTemplate>>;