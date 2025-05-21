using Domain.Abstractions.Result;
using Domain.Entities.Services;
using MediatR;

namespace Application.Features.Services.Queries;

public record GetServicesByResultTemplateIdQuery(int ResultTemplateId) : IRequest<Result<ICollection<Service>>>;