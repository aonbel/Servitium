using Application.Features.Specialists.Queries;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Models.Entities.People;
using MediatR;

namespace Application.Features.Specialists.Handlers;

public sealed class GetSpecialistQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetSpecialistQuery, Result<Specialist>>
{
    public async Task<Result<Specialist>> Handle(GetSpecialistQuery request, CancellationToken cancellationToken)
    {
        var specialist = await applicationDbContext.Specialists.FindAsync([request.Id], cancellationToken);

        if (specialist is null)
        {
            return new Error("SpecialistNotFound", $"Specialist with given id {request.Id} does not exist");
        }
        
        return specialist;
    }
}