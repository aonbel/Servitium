using Domain.Abstractions.Result;
using Domain.Entities.Services;
using MediatR;

namespace Application.Features.HealthСertificatates.Queries;

public record GetHealthCertificateByIdQuery(int Id) : IRequest<Result<HealthCertificate>>;