using Application.Features.ServiceProviders.Queries;
using Application.Features.Specialists.Queries;
using MediatR;

namespace Servitium.Endpoints.Specialists;

public class FromServiceProviderCanProvideServiceAt : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/Specialists/FromServiceProvider/{serviceProviderId:int}/CanProvideService/{serviceId:int}/At/{date}/After/{time}", async (
            int serviceProviderId,
            int serviceId,
            DateOnly date,
            TimeOnly time,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var dateTime = new DateTime(date, time);
            
            var getAllSpecialistsByServiceProviderIdAndServiceIdAndDateOnlyQuery =
                new GetAllSpecialistsByServiceProviderIdAndServiceIdAndDateTimeQuery(serviceProviderId, serviceId, dateTime);

            var getAllSpecialistsByServiceProviderIdAndServiceIdAndDateOnlyQueryResponse =
                await sender.Send(getAllSpecialistsByServiceProviderIdAndServiceIdAndDateOnlyQuery, cancellationToken);
            
            return getAllSpecialistsByServiceProviderIdAndServiceIdAndDateOnlyQueryResponse;
        });
    }
}