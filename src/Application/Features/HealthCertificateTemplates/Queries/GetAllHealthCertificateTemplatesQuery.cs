using Domain.Abstractions.Result;
using Domain.Entities.Services;
using MediatR;

namespace Application.Features.HealthCertificateTemplates.Queries;

public sealed record GetAllHealthCertificateTemplatesQuery : IRequest<Result<ICollection<HealthCertificateTemplate>>>;