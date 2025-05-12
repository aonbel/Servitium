using Application.Features.Appointments.Queries;
using Domain.Entities.Services;
using Infrastructure.Authentification;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Servitium.Pages.Appointments;

[Authorize(Roles = "Client")]
public class Index(ISender sender) : PageModel
{
    public ICollection<Appointment> Appointments { get; set; } = [];
    
    public async Task<IActionResult> OnGetAsync()
    {
        var userId = User.GetUserId();
        
        var query = new GetAllAppointmentsByClientIdQuery(userId);

        var response = await sender.Send(query);

        if (response.IsError)
        {
            ModelState.AddModelError(response.Error.Code, response.Error.Message);
            return LocalRedirect("/Index");
        }

        Appointments = response.Value;

        return Page();
    }
}