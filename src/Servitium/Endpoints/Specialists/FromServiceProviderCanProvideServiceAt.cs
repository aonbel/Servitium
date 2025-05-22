using Application.Features.ServiceProviders.Queries;
using Application.Features.Specialists.Queries;
using MediatR;

namespace Servitium.Endpoints.Specialists;

public class FromServiceProviderCanProvideServiceAt : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/Specialists/FromServiceProvider/{serviceProviderId:int}/CanProvideService/{serviceId:int}/At/{date}", async (
            int serviceProviderId,
            int serviceId,
            DateOnly date,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var getAllSpecialistsByServiceProviderIdAndServiceIdAndDateOnlyQuery =
                new GetAllSpecialistsByServiceProviderIdAndServiceIdAndDateOnlyQuery(serviceProviderId, serviceId, date);

            var getAllSpecialistsByServiceProviderIdAndServiceIdAndDateOnlyQueryResponse =
                await sender.Send(getAllSpecialistsByServiceProviderIdAndServiceIdAndDateOnlyQuery, cancellationToken);
            
            return getAllSpecialistsByServiceProviderIdAndServiceIdAndDateOnlyQueryResponse;
        });
    }
}