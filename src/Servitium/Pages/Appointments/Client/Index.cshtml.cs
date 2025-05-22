using Application.Features.Appointments.Queries;
using Application.Features.Clients.Queries;
using Application.Features.Persons.Queries;
using Application.Features.ServiceProviders.Queries;
using Application.Features.Services.Queries;
using Application.Features.Specialists.Queries;
using Domain.Entities.Services;
using Infrastructure.Authentification;
using Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Servitium.Extensions;
using ServiceProvider = Domain.Entities.Services.ServiceProvider;

namespace Servitium.Pages.Appointments.Client;

[Authorize(Roles = ApplicationRoles.Client)]
public class Index(ISender sender) : PageModel
{
    public ICollection<(Appointment appointment, ServiceProvider serviceProvider, Service service)> Data
    {
        get;
        set;
    } = [];

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
            return RedirectToPage(Routes.Index);
        }

        var person = getPersonByUserIdQueryResponse.Value;

        var getClientByPersonIdQuery = new GetClientByPersonIdQuery(person.Id ?? 0);

        var getClientByPersonIdQueryResponse = await sender.Send(getClientByPersonIdQuery);

        if (getClientByPersonIdQueryResponse.IsError)
        {
            ModelState.AddModelError(
                getClientByPersonIdQueryResponse.Error.Code,
                getClientByPersonIdQueryResponse.Error.Message);
            return RedirectToPage(Routes.Index);
        }

        var client = getClientByPersonIdQueryResponse.Value;

        var getAllAppointmentsByClientIdQuery = new GetAllAppointmentsByClientIdQuery(client.Id ?? 0);

        var getAllAppointmentsByClientIdQueryResponse = await sender.Send(getAllAppointmentsByClientIdQuery);

        if (getAllAppointmentsByClientIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getAllAppointmentsByClientIdQueryResponse.Error);
            return RedirectToPage(Routes.Index);
        }

        var appointments = getAllAppointmentsByClientIdQueryResponse.Value;

        foreach (var appointment in appointments)
        {
            var getSpecialistByIdQuery = new GetSpecialistByIdQuery(appointment.SpecialistId);

            var getSpecialistByIdQueryResponse = await sender.Send(getSpecialistByIdQuery);

            if (getSpecialistByIdQueryResponse.IsError)
            {
                ModelState.AddModelError(getSpecialistByIdQueryResponse.Error);
                return RedirectToPage(Routes.Index);
            }

            var specialist = getSpecialistByIdQueryResponse.Value;

            var getServiceProviderByIdQuery = new GetServiceProviderByIdQuery(specialist.ServiceProviderId);

            var getServiceProviderByIdQueryResponse = await sender.Send(getServiceProviderByIdQuery);

            if (getServiceProviderByIdQueryResponse.IsError)
            {
                ModelState.AddModelError(getServiceProviderByIdQueryResponse.Error);
                return RedirectToPage(Routes.Index);
            }
            
            var serviceProvider = getServiceProviderByIdQueryResponse.Value;
            
            var getServiceByIdQuery = new GetServiceByIdQuery(appointment.ServiceId);
            
            var getServiceByIdQueryResponse = await sender.Send(getServiceByIdQuery);

            if (getServiceByIdQueryResponse.IsError)
            {
                ModelState.AddModelError(getServiceByIdQueryResponse.Error);
                return RedirectToPage(Routes.Index);
            }
            
            var service = getServiceByIdQueryResponse.Value;

            Data.Add((appointment, serviceProvider, service));
        }

        return Page();
    }
}