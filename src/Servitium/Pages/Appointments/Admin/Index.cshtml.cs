using Application.Features.Appointments.Queries;
using Domain.Entities.Services;
using Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Servitium.Pages.Appointments.Admin;

[Authorize(Roles = ApplicationRoles.Admin)]
public class All(ISender sender) : PageModel
{
    public ICollection<Appointment> Appointments { get; set; } = [];

    public class DataModel
    {
        
    }
    
    public async Task<IActionResult> OnGetAsync()
    {
        var query = new GetAllAppointmentsQuery();

        var result = await sender.Send(query);

        if (result.IsError)
        {
            ModelState.AddModelError(result.Error.Code, result.Error.Message);
            return LocalRedirect(Routes.Index);
        }

        Appointments = result.Value;

        return Page();
    }
}