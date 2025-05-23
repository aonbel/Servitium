using Application.Features.HealthСertificatates.Responses;
using Domain.Abstractions.Result;
using MediatR;

namespace Application.Features.HealthСertificatates.Queries;

public sealed record GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQuery(int ClientId, int HealthCertificateTemplateId)
    : IRequest<Result<GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryResponse>>;