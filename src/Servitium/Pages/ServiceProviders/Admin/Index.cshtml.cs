using Application.Features.ServiceProviders.Queries;
using Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceProvider = Domain.Entities.Services.ServiceProvider;

namespace Servitium.Pages.ServiceProviders.Admin;

[Authorize(Roles = ApplicationRoles.Admin)]
public class Index(ISender sender) : PageModel
{
    public ICollection<ServiceProvider> ServicesProviders { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var query = new GetAllServiceProvidersQuery();
        
        var response = await sender.Send(query);

        if (response.IsError)
        {
            ModelState.AddModelError(response.Error.Code, response.Error.Message);
            return LocalRedirect(Routes.Index);
        }

        ServicesProviders = response.Value;
        
        return Page();
    }
}