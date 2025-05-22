using Application.Features.Appointments.Queries;
using Application.Features.Persons.Queries;
using MediatR;

namespace Servitium.Endpoints.Appointments;

public class GetAllFreeTimeSegmentsBySpecialistIdToProvideServiceServiceIdAt : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet(
            "api/Appointments/GetAllFreeTimeSegmentsBySpecialistId/{specialistId:int}/ToProvideService/{serviceId:int}/At/{date}",
            async (
                int specialistId,
                int serviceId,
                DateOnly date,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var getAllFreeAppointmentsTimeOnlySegmentsBySpecialistIdAndServiceIdAndDateOnlyQuery =
                    new GetAllFreeAppointmentsTimeOnlySegmentsBySpecialistIdAndServiceIdAndDateOnlyQuery(
                        specialistId,
                        serviceId,
                        date);

                var getAllFreeAppointmentsTimeOnlySegmentsBySpecialistIdAndServiceIdAndDateOnlyQueryResponse =
                    await sender.Send(
                        getAllFreeAppointmentsTimeOnlySegmentsBySpecialistIdAndServiceIdAndDateOnlyQuery,
                        cancellationToken);

                return getAllFreeAppointmentsTimeOnlySegmentsBySpecialistIdAndServiceIdAndDateOnlyQueryResponse;
            });
    }
}