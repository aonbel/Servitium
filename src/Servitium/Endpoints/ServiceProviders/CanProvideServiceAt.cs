using Application.Features.ServiceProviders.Queries;
using MediatR;

namespace Servitium.Endpoints.ServiceProviders;

public class CanProvideServiceAt : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/ServiceProviders/CanProvideService/{serviceId:int}/At/{date}", async (
            int serviceId,
            DateOnly date,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var getAllServiceProvidersByServiceAndDateOnlyQuery =
                new GetAllServiceProvidersByServiceAndDateOnlyQuery(serviceId, date);

            var getAllServiceProvidersByServiceAndDateOnlyQueryResponse =
                await sender.Send(getAllServiceProvidersByServiceAndDateOnlyQuery, cancellationToken);
            
            return getAllServiceProvidersByServiceAndDateOnlyQueryResponse;
        });
    }
}