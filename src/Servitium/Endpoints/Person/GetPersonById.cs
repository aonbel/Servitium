using Application.Features.Persons.Queries;
using Application.Features.ServiceProviders.Queries;
using MediatR;

namespace Servitium.Endpoints.Person;

public class GetPersonById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/Person/GetById/{personId:int}", async (
            int personId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var getPersonByIdQuery = new GetPersonByIdQuery(personId);

            var getPersonByIdQueryResponse = await sender.Send(getPersonByIdQuery, cancellationToken);

            return getPersonByIdQueryResponse;
        });
    }
}