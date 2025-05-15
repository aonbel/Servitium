using Application.Features.Appointments.Queries;
using Application.Features.Clients.Queries;
using Application.Features.Persons.Queries;
using Domain.Entities.Services;
using Infrastructure.Authentification;
using Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Servitium.Pages.Appointments;

[Authorize(Roles = ApplicationRoles.Client)]
public class Index(ISender sender) : PageModel
{
    public ICollection<Appointment> Appointments { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = User.GetUserId();

        var getPersonByUserIdQuery = new GetPersonByUserIdQuery(userId);

        var getPersonByUserIdQueryResponse = await sender.Send(getPersonByUserIdQuery);

        if (getPersonByUserIdQueryResponse.IsError)
        {
            ModelState.AddModelError(
                getPersonByUserIdQueryResponse.Error.Code,
                getPersonByUserIdQueryResponse.Error.Message);
            return LocalRedirect(Routes.Index);
        }

        var person = getPersonByUserIdQueryResponse.Value;

        var getClientByPersonIdQuery = new GetClientByPersonIdQuery(person.Id ?? 0);

        var getClientByPersonIdQueryResponse = await sender.Send(getClientByPersonIdQuery);

        if (getClientByPersonIdQueryResponse.IsError)
        {
            ModelState.AddModelError(
                getClientByPersonIdQueryResponse.Error.Code,
                getClientByPersonIdQueryResponse.Error.Message);
            return LocalRedirect(Routes.Index);
        }
        
        var client = getClientByPersonIdQueryResponse.Value;

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