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
    public DataModel Data { get; set; } = new();

    public class DataModel
    {
        public int? CurrentAppointmentId { get; set; }
        
        public List<Appointment> Appointments { get; set; } = [];
        
        public List<Service> Services { get; set; } = [];
    }
    
    public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
    {
        returnUrl ??= Routes.Index;
        
        var userId = User.GetUserId();
        
        var getSpecialistByUserIdQuery = new GetSpecialistByUserIdQuery(userId);
        
        var getSpecialistByUserIdQueryResponse = await sender.Send(getSpecialistByUserIdQuery);

        if (getSpecialistByUserIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getSpecialistByUserIdQueryResponse.Error);
            return LocalRedirect(returnUrl);
        }
        
        var specialist = getSpecialistByUserIdQueryResponse.Value;

        var getAllAppointmentsBySpecialistIdQuery = new GetAllAppointmentsBySpecialistIdQuery(specialist.Id ?? 0);

        var getAllAppointmentsBySpecialistIdQueryResponse = await sender.Send(getAllAppointmentsBySpecialistIdQuery);

        if (getAllAppointmentsBySpecialistIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getAllAppointmentsBySpecialistIdQueryResponse.Error);
            return LocalRedirect(returnUrl);
        }

        var appointments = getAllAppointmentsBySpecialistIdQueryResponse.Value;

        foreach (var appointment in appointments)
        {
            var getServiceByIdQuery = new GetServiceByIdQuery(appointment.ServiceId);
            
            var getServiceByIdQueryResponse = await sender.Send(getServiceByIdQuery);

            if (getServiceByIdQueryResponse.IsError)
            {
                ModelState.AddModelError(getServiceByIdQueryResponse.Error);
                return LocalRedirect(returnUrl);
            }
            
            var service = getServiceByIdQueryResponse.Value;
            
            Data.Appointments.Add(appointment);
            Data.Services.Add(service);
        }
        
        var getAppointmentBySpecialistIdAndDateTimeQuery = new GetAppointmentBySpecialistIdAndDateTimeQuery(specialist.Id ?? 0, DateTime.UtcNow);
        
        var getAppointmentBySpecialistIdAndDateTimeQueryResponse = await sender.Send(getAppointmentBySpecialistIdAndDateTimeQuery);

        if (getAppointmentBySpecialistIdAndDateTimeQueryResponse.IsError)
        {
            ModelState.AddModelError(getAppointmentBySpecialistIdAndDateTimeQueryResponse.Error);
            return LocalRedirect(returnUrl);
        }
        
        var currentAppointment = getAppointmentBySpecialistIdAndDateTimeQueryResponse.Value.Appointment;

        if (currentAppointment is not null)
        {
            Data.CurrentAppointmentId = currentAppointment.Id;
        }
        
        return Page();
    }
}