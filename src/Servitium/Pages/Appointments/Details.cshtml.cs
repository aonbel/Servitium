using Application.Features.Appointments.Queries;
using Application.Features.ServiceProviders.Queries;
using Application.Features.Services.Queries;
using Application.Features.Specialists.Queries;
using Domain.Entities.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Servitium.Extensions;

namespace Servitium.Pages.Appointments;

public class Details(ISender sender) : PageModel
{
    public string? ReturnUrl { get; set; } = Routes.Index;
    
    public AppointmentDetails Data { get; set; } = new();
    
    public class AppointmentDetails
    {
        public string SpecialistLocation { get; set; } = string.Empty;
        
        public string ServiceProviderName { get; set; } = string.Empty;
        
        public string ServiceName { get; set; } = string.Empty;
        
        public DateOnly Date { get; set; }
        
        public TimeOnlySegment TimeSegment { get; set; }
    }
    
    public async Task<IActionResult> OnGetAsync(int id, string? returnUrl = null)
    {
        if (returnUrl is not null)
        {
            ReturnUrl = returnUrl;
        }
        
        var getAppointmentByIdQuery = new GetAppointmentByIdQuery(id);
        
        var getAppointmentByIdQueryResponse = await sender.Send(getAppointmentByIdQuery);

        if (getAppointmentByIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getAppointmentByIdQueryResponse.Error);
            return LocalRedirect(Routes.Index);
        }
        
        var appointment = getAppointmentByIdQueryResponse.Value;
        
        var getSpecialistByIdQuery = new GetSpecialistByIdQuery(appointment.SpecialistId);
        
        var getSpecialistByIdQueryResponse = await sender.Send(getSpecialistByIdQuery);

        if (getSpecialistByIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getSpecialistByIdQueryResponse.Error);
            return LocalRedirect(Routes.Index);
        }
        
        var specialist = getSpecialistByIdQueryResponse.Value;
        
        var getServiceProviderByIdQuery = new GetServiceProviderByIdQuery(specialist.ServiceProviderId);
        
        var getServiceProviderByIdQueryResponse = await sender.Send(getServiceProviderByIdQuery);

        if (getServiceProviderByIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getServiceProviderByIdQueryResponse.Error);
            return LocalRedirect(Routes.Index);
        }
        
        var serviceProvider = getServiceProviderByIdQueryResponse.Value;
        
        var getServiceByIdQuery = new GetServiceByIdQuery(appointment.ServiceId);
        
        var getServiceByIdQueryResponse = await sender.Send(getServiceByIdQuery);

        if (getServiceByIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getServiceByIdQueryResponse.Error);
            return LocalRedirect(Routes.Index);
        }
        
        var service = getServiceByIdQueryResponse.Value;

        Data = new AppointmentDetails
        {
            SpecialistLocation = specialist.Location,
            ServiceProviderName = service.Name,
            ServiceName = service.Name,
            Date = appointment.Date,
            TimeSegment = appointment.TimeSegment
        };

        return Page();
    }
}