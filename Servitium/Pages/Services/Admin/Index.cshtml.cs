using Application.Features.Services.Queries;
using Domain.Entities.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Servitium.Extensions;

namespace Servitium.Pages.Services.Admin;

public class Index(ISender sender) : PageModel
{
    public ICollection<Service> Data { get; set; } = [];
    
    public async Task<IActionResult> OnGetAsync()
    {
        var getAllServicesQuery = new GetAllServicesQuery();
        
        var getAllServicesQueryResponse = await sender.Send(getAllServicesQuery);

        if (getAllServicesQueryResponse.IsError)
        {
            ModelState.AddModelError(getAllServicesQueryResponse.Error);
            
            return Page();
        }

        var services = getAllServicesQueryResponse.Value;

        Data = services;
        
        return Page();
    }
}