using Application.Features.Appointments.Queries;
using Domain.Entities.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Servitium.Pages.Appointments;

[Authorize(Roles = "Admin")]
public class All(ISender sender) : PageModel
{
    public ICollection<Appointment> Appointments { get; set; } = [];
    
    public async Task<IActionResult> OnGetAsync()
    {
        var query = new GetAllAppointmentsQuery();

        var result = await sender.Send(query);

        if (result.IsError)
        {
            ModelState.AddModelError(result.Error.Code, result.Error.Message);
            return LocalRedirect("/Index");
        }

        Appointments = result.Value;

        return Page();
    }
}