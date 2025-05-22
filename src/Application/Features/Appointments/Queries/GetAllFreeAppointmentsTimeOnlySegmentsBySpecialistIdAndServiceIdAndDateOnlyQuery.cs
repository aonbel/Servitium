using Domain.Abstractions.Result;
using Domain.Entities.Core;
using MediatR;

namespace Application.Features.Appointments.Queries;

public record GetAllFreeAppointmentsTimeOnlySegmentsBySpecialistIdAndServiceIdAndDateOnlyQuery(int SpecialistId, int ServiceId, DateOnly Date) :
    IRequest<Result<ICollection<TimeOnlySegment>>>;