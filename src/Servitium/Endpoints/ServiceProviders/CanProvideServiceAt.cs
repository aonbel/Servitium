using Application.Features.ServiceProviders.Queries;
using MediatR;

namespace Servitium.Endpoints.ServiceProviders;

public class CanProvideServiceAt : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/ServiceProviders/CanProvideService/{serviceId:int}/At/{date}/After/{time}", async (
            int serviceId,
            DateOnly date,
            TimeOnly time,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var dateTime = new DateTime(date, time);
            
            var getAllServiceProvidersByServiceAndDateOnlyQuery =
                new GetAllServiceProvidersByServiceAndDateTimeQuery(serviceId, dateTime);

            var getAllServiceProvidersByServiceAndDateOnlyQueryResponse =
                await sender.Send(getAllServiceProvidersByServiceAndDateOnlyQuery, cancellationToken);
            
            return getAllServiceProvidersByServiceAndDateOnlyQueryResponse;
        });
    }
}