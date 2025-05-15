using System.ComponentModel.DataAnnotations;
using Application.Features.Persons.Queries;
using Application.Features.ServiceProviderManagers.Commands;
using Application.Features.ServiceProviders.Queries;
using Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Servitium.Pages.ServiceProviderManagers;

[Authorize(Roles = ApplicationRoles.Admin)]
public class Create(ISender sender) : PageModel
{
    public SelectList ServiceProviders { get; set; }

    [BindProperty] public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required] public int ServiceProviderId { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var query = new GetAllServiceProvidersQuery();

        var response = await sender.Send(query);

        if (response.IsError)
        {
            ModelState.AddModelError(response.Error.Code, response.Error.Message);
            return LocalRedirect(Routes.Index);
        }

        var serviceProviders = response.Value;

        ServiceProviders = new SelectList(serviceProviders, "Id", "Name");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id, string? returnUrl = null)
    {
        returnUrl ??= Url.Content(Routes.Index);

        var getPersonByUserIdQuery = new GetPersonByUserIdQuery(id);

        var getPersonByUserIdQueryResponse = await sender.Send(getPersonByUserIdQuery);

        if (getPersonByUserIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getPersonByUserIdQueryResponse.Error.Code,
                getPersonByUserIdQueryResponse.Error.Message);
            return Page();
        }

        var person = getPersonByUserIdQueryResponse.Value;

        var createServiceProviderManagerCommand =
            new CreateServiceProviderManagerCommand(person.Id ?? 0, Input.ServiceProviderId);
        
        var createServiceProviderManagerCommandResponse = await sender.Send(createServiceProviderManagerCommand);

        if (createServiceProviderManagerCommandResponse.IsError)
        {
            ModelState.AddModelError(createServiceProviderManagerCommandResponse.Error.Code,
                createServiceProviderManagerCommandResponse.Error.Message);
            return Page();
        }
        
        return LocalRedirect(returnUrl);
    }
}