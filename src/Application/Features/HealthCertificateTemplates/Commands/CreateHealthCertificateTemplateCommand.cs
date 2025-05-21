using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.Services;
using MediatR;

namespace Application.Features.HealthCertificateTemplates.Commands;

public sealed record CreateHealthCertificateTemplateCommand(
    string Name,
    TimeSpan ActivePeriod) : IRequest<Result<HealthCertificateTemplate>>;