using Domain.Entities.Services;

namespace Application.Features.Appointments.Response;

public record GetAllPossibleAppointmentTimesQueryResponse(
    ICollection<(int ServiceProviderId, int SpecialistId, DateOnly Date, TimeOnly Time)> PossibleTimes);