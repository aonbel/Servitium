using Application.Features.Persons.Queries;
using Application.Features.ServiceProviderManagers.Queries;
using Application.Features.ServiceProviders.Queries;
using Domain.Entities.People;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceProvider = Domain.Entities.Services.ServiceProvider;

namespace Servitium.Pages.ServiceProviderManagers;

public class Index(ISender sender) : PageModel
{
    public ICollection<(ServiceProviderManager manager, Person person, ServiceProvider serviceProvider)> Data
    {
        get;
        set;
    } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var getAllServiceProviderManagersQuery = new GetAllServiceProviderManagersQuery();

        var getAllServiceProviderManagersQueryResponse = await sender.Send(getAllServiceProviderManagersQuery);

        if (getAllServiceProviderManagersQueryResponse.IsError)
        {
            ModelState.AddModelError(
                getAllServiceProviderManagersQueryResponse.Error.Code,
                getAllServiceProviderManagersQueryResponse.Error.Message);
            return LocalRedirect(Routes.Index);
        }
        
        var serviceProviderManagers = getAllServiceProviderManagersQueryResponse.Value;

        foreach (var manager in serviceProviderManagers)
        {
            var getServiceProviderByIdQuery = new GetServiceProviderByIdQuery(manager.ServiceProviderId);
            
            var getServiceProviderByIdQueryResponse = await sender.Send(getServiceProviderByIdQuery);

            if (getServiceProviderByIdQueryResponse.IsError)
            {
                ModelState.AddModelError(
                    getServiceProviderByIdQueryResponse.Error.Code,
                    getServiceProviderByIdQueryResponse.Error.Message);
                return LocalRedirect(Routes.Index);
            }
            
            var serviceProvider = getServiceProviderByIdQueryResponse.Value;
            
            var getPersonByIdQuery = new GetPersonByIdQuery(manager.PersonId);
            
            var getPersonByIdQueryResponse = await sender.Send(getPersonByIdQuery);

            if (getPersonByIdQueryResponse.IsError)
            {
                ModelState.AddModelError(
                    getPersonByIdQueryResponse.Error.Code,
                    getPersonByIdQueryResponse.Error.Message);
                return LocalRedirect(Routes.Index);
            }
            
            var person = getPersonByIdQueryResponse.Value;
            
            Data.Add((manager, person, serviceProvider));
        }
        
        return Page();
    }
}