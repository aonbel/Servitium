using Domain.Abstractions;
using Domain.Models.Entities.Core;
using MediatR;

namespace Application.Features.HealthСertificatates.Commands;

public sealed record CreateHealthCertificateCommand(
    int TemplateId,
    int ClientId,
    string Description) : IRequest<Result<int>>;