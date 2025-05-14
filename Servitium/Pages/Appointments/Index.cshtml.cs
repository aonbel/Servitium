using Application.Features.Appointments.Queries;
using Application.Features.Clients.Queries;
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

        var getClientByUserIdQuery = new GetClientByUserIdQuery(userId);

        var getClientByUserIdQueryResponse = await sender.Send(getClientByUserIdQuery);

        if (getClientByUserIdQueryResponse.IsError)
        {
            ModelState.AddModelError(
                getClientByUserIdQueryResponse.Error.Code,
                getClientByUserIdQueryResponse.Error.Message);
            return LocalRedirect(Routes.Index);
        }
        
        var client = getClientByUserIdQueryResponse.Value;

        var getAllAppointmentsByClientIdQuery = new GetAllAppointmentsByClientIdQuery(client.Id ?? 0);

        var response = await sender.Send(getAllAppointmentsByClientIdQuery);

        if (response.IsError)
        {
            ModelState.AddModelError(response.Error.Code, response.Error.Message);
            return LocalRedirect(Routes.Index);
        }

        Appointments = response.Value;

        return Page();
    }
}