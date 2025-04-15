using Domain.Abstractions;
using Domain.Models.Entities.Services;
using MediatR;

namespace Application.Features.HealthСertificatates.Queries;

public sealed record GetLatestHealthCertificateQuery(int ClientId, int HealthCertificateTemplateId)
    : IRequest<Result<HealthCertificate>>;