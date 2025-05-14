using Application.Features.Specialists.Queries;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.People;
using Infrastructure.Interfaces;
using MediatR;

namespace Application.Features.Specialists.Handlers;

public sealed class GetSpecialistByIdQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetSpecialistByIdQuery, Result<Specialist>>
{
    public async Task<Result<Specialist>> Handle(GetSpecialistByIdQuery request, CancellationToken cancellationToken)
    {
        var specialist = await applicationDbContext.Specialists.FindAsync([request.Id], cancellationToken);

        if (specialist is null)
        {
            return new Error("SpecialistNotFound", $"Specialist with given id {request.Id} does not exist");
        }
        
        return specialist;
    }
}