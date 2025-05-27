using Application.Features.HealthCertificateTemplates.Responces;
using Domain.Abstractions.Result;
using MediatR;

namespace Application.Features.HealthCertificateTemplates.Queries;

public record GetNeededHealthCertificateTemplatesByHealthCertificateTemplateIdAndClientIdQuery(
    int HealthCertificateTemplateId,
    int ClientId) : IRequest<Result<GetNeededHealthCertificateTemplatesByHealthCertificateTemplateIdAndClientIdQueryResponse>>;