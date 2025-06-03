using Domain.Abstractions.Result;
using MediatR;

namespace Application.Features.HealthСertificatates.Commands;

public record UpdateHealthCertificateStatusCommand(
    int CertificateId,
    string Description) : IRequest<Result>;