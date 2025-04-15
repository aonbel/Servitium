using Domain.Abstractions;
using Domain.Models.Entities.Services;
using MediatR;

namespace Application.Features.HealthCertificateTemplates.Queries;

public sealed record GetAllHealthCertificateTemplates : IRequest<Result<ICollection<HealthCertificateTemplate>>>;