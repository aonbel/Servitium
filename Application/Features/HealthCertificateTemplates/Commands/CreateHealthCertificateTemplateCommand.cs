using Domain.Abstractions;
using MediatR;

namespace Application.Features.HealthCertificateTemplates.Commands;

public sealed record CreateHealthCertificateTemplateCommand(
    string Name,
    TimeSpan ActivePeriod) : IRequest<Result<int>>;