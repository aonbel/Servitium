using Application.Features.Appointments.Queries;
using Application.Features.Persons.Queries;
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

namespace Servitium.Pages.Appointments.Specialist;

[Authorize(Roles = ApplicationRoles.Specialist)]
public class Index(ISender sender) : PageModel
{
    public int? CurrentAppointmentId { get; set; }
    public ICollection<(Appointment appointment, Service service)> Data { get; set; } = [];
    public async Task<IActionResult> OnGetAsync()
    {
        var userId = User.GetUserId();

        var getPersonByUserIdQuery = new GetPersonByUserIdQuery(userId);

        var getPersonByUserIdQueryResponse = await sender.Send(getPersonByUserIdQuery);

        if (getPersonByUserIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getPersonByUserIdQueryResponse.Error);
            return RedirectToPage(Routes.Index);
        }

        var person = getPersonByUserIdQueryResponse.Value;

        var getSpecialistByPersonIdQuery = new GetSpecialistByPersonIdQuery(person.Id ?? 0);

        var getSpecialistByPersonIdQueryResponse = await sender.Send(getSpecialistByPersonIdQuery);

        if (getSpecialistByPersonIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getSpecialistByPersonIdQueryResponse.Error);
            return RedirectToPage(Routes.Index);
        }

        var specialist = getSpecialistByPersonIdQueryResponse.Value;

        var getAllAppointmentsBySpecialistIdQuery = new GetAllAppointmentsBySpecialistIdQuery(specialist.Id ?? 0);

        var getAllAppointmentsBySpecialistIdQueryResponse = await sender.Send(getAllAppointmentsBySpecialistIdQuery);

        if (getAllAppointmentsBySpecialistIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getAllAppointmentsBySpecialistIdQueryResponse.Error);
            return RedirectToPage(Routes.Index);
        }

        var appointments = getAllAppointmentsBySpecialistIdQueryResponse.Value;
        
        var currentDateTime = DateTime.Now;
        var currentDateOnly = DateOnly.FromDateTime(currentDateTime);
        var currentTimeOnly = TimeOnly.FromDateTime(currentDateTime);

        foreach (var appointment in appointments)
        {
            var getServiceByIdQuery = new GetServiceByIdQuery(appointment.ServiceId);

            var getServiceByIdQueryResponse = await sender.Send(getServiceByIdQuery);

            if (getServiceByIdQueryResponse.IsError)
            {
                ModelState.AddModelError(getServiceByIdQueryResponse.Error);
                return RedirectToPage(Routes.Index);
            }

            var service = getServiceByIdQueryResponse.Value;

            Data.Add((appointment, service));

            if (appointment.Date == currentDateOnly && appointment.TimeSegment.Contains(currentTimeOnly))
            {
                CurrentAppointmentId = appointment.Id;
            }
        }

        return Page();
    }
}