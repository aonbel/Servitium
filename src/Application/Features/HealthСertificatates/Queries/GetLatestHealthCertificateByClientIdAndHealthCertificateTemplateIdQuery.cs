using Domain.Abstractions.Result;
using Domain.Entities.Services;
using MediatR;

namespace Application.Features.HealthСertificatates.Queries;

public sealed record GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQuery(int ClientId, int HealthCertificateTemplateId)
    : IRequest<Result<HealthCertificate>>;