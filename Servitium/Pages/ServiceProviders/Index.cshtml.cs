using Application.Features.ServiceProviders.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceProvider = Domain.Entities.Services.ServiceProvider;

namespace Servitium.Pages.ServiceProviders;

public class Index(ISender sender) : PageModel
{
    public ICollection<ServiceProvider> ServicesProviders { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        if (User.IsInRole("Admin"))
        {
            return LocalRedirect(Routes.ServiceProvidersAdminIndex);
        }
        
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