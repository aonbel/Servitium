using Application.Features.Appointments.Queries;
using Application.Features.Clients.Queries;
using Application.Features.Persons.Queries;
using Application.Features.Services.Queries;
using Domain.Entities.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Servitium.Endpoints.Person;
using Servitium.Extensions;

namespace Servitium.Pages.Appointments.Specialist;

public class Details(ISender sender) : PageModel
{
    public DataModel Data { get; set; } = new ();
    
    public int ClientId { get; set; }
    
    public int ServiceId { get; set; }
    
    public class DataModel
    {
        public DateOnly AppointmentDate { get; set; }

        public TimeOnlySegment AppointmentTime { get; set; } = new();
        
        public string ServiceName { get; set; } = string.Empty;
        
        public string ClientName { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync(int appointmentId)
    {
        var getAppointmentByIdQuery = new GetAppointmentByIdQuery(appointmentId);
        
        var getAppointmentByIdQueryResponse = await sender.Send(getAppointmentByIdQuery);

        if (getAppointmentByIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getAppointmentByIdQueryResponse.Error);
            return RedirectToPage(Routes.AppointmentSpecialistIndex);
        }

        var appointment = getAppointmentByIdQueryResponse.Value;
        
        ServiceId = appointment.ServiceId;
        
        var getServiceByIdQuery = new GetServiceByIdQuery(appointment.ServiceId);
        
        var getServiceByIdQueryResponse = await sender.Send(getServiceByIdQuery);

        if (getServiceByIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getServiceByIdQueryResponse.Error);
            return RedirectToPage(Routes.AppointmentSpecialistIndex);
        }
        
        var service = getServiceByIdQueryResponse.Value;
        
        var getClientByIdQuery = new GetClientByIdQuery(appointment.ClientId);
        
        var getClientByIdQueryResponse = await sender.Send(getClientByIdQuery);

        if (getClientByIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getClientByIdQueryResponse.Error);
            return RedirectToPage(Routes.AppointmentSpecialistIndex);
        }
        
        var client = getClientByIdQueryResponse.Value;
        
        var getPersonByIdQuery = new GetPersonByIdQuery(client.PersonId);
        
        var getPersonByIdQueryResponse = await sender.Send(getPersonByIdQuery);

        if (getPersonByIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getPersonByIdQueryResponse.Error);
            return RedirectToPage(Routes.AppointmentSpecialistIndex);
        }
        
        var clientPerson = getPersonByIdQueryResponse.Value;

        Data = new DataModel
        {
            AppointmentDate = appointment.Date,
            AppointmentTime = appointment.TimeSegment,
            ServiceName = service.Name,
            ClientName = $"{clientPerson.LastName} {clientPerson.FirstName}"
        };

        ClientId = client.Id ?? 0;
        
        return Page();
    }
}