using Application.Features.Persons.Queries;
using Domain.Entities.People;
using Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Servitium.Pages.Persons.Manager;

[Authorize(Roles = ApplicationRoles.Manager)]
public class Index(ISender sender) : PageModel
{
    public ICollection<Person> Data { get; set; } = [];
    
    public async Task<IActionResult> OnGetAsync()
    {
        var query = new GetAllPersonsQuery();
        
        var response = await sender.Send(query);

        if (response.IsError)
        {
            ModelState.AddModelError(response.Error.Code, response.Error.Message);
            return LocalRedirect(Routes.Index);
        }
        
        Data = response.Value;
        
        return Page();
    }
}