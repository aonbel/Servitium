using Domain.Abstractions.Result;
using Domain.Entities.Services;
using MediatR;

namespace Application.Features.HealthСertificatates.Queries;

public record GetHealthCertificatesByClientIdQuery(int ClientId) : IRequest<Result<ICollection<HealthCertificate>>>;