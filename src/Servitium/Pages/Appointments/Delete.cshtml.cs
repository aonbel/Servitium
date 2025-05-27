using System.ComponentModel.DataAnnotations;
using Application.Features.Appointments.Commands;
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

public class Delete(ISender sender) : PageModel
{
    public string ReturnUrl { get; set; } = Routes.Index;
    
    public DataModel Data { get; set; } = new();
    
    public class DataModel
    {
        [Display(Name = "Location of the specialist in service provider")]
        public string SpecialistLocation { get; set; } = string.Empty;
        
        [Display(Name = "Service provider name")]
        public string ServiceProviderName { get; set; } = string.Empty;
        
        [Display(Name = "Service name")]
        public string ServiceName { get; set; } = string.Empty;
        
        [Display(Name = "Date of appointment")]
        public DateOnly Date { get; set; }

        [Display(Name = "Time of appointment")]
        public TimeOnlySegment TimeSegment { get; set; } = new();
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
            return LocalRedirect(ReturnUrl);
        }
        
        var appointment = getAppointmentByIdQueryResponse.Value;
        
        var getSpecialistByIdQuery = new GetSpecialistByIdQuery(appointment.SpecialistId);
        
        var getSpecialistByIdQueryResponse = await sender.Send(getSpecialistByIdQuery);

        if (getSpecialistByIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getSpecialistByIdQueryResponse.Error);
            return LocalRedirect(ReturnUrl);
        }
        
        var specialist = getSpecialistByIdQueryResponse.Value;
        
        var getServiceProviderByIdQuery = new GetServiceProviderByIdQuery(specialist.ServiceProviderId);
        
        var getServiceProviderByIdQueryResponse = await sender.Send(getServiceProviderByIdQuery);

        if (getServiceProviderByIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getServiceProviderByIdQueryResponse.Error);
            return LocalRedirect(ReturnUrl);
        }
        
        var serviceProvider = getServiceProviderByIdQueryResponse.Value;
        
        var getServiceByIdQuery = new GetServiceByIdQuery(appointment.ServiceId);
        
        var getServiceByIdQueryResponse = await sender.Send(getServiceByIdQuery);

        if (getServiceByIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getServiceByIdQueryResponse.Error);
            return LocalRedirect(ReturnUrl);
        }
        
        var service = getServiceByIdQueryResponse.Value;

        Data = new DataModel
        {
            SpecialistLocation = specialist.Location,
            ServiceProviderName = serviceProvider.Name,
            ServiceName = service.Name,
            Date = appointment.Date,
            TimeSegment = appointment.TimeSegment
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var deleteAppointmentByIdCommand = new DeleteAppointmentByIdCommand(id);
        
        var deleteAppointmentByIdResponse = await sender.Send(deleteAppointmentByIdCommand);

        if (deleteAppointmentByIdResponse.IsError)
        {
            ModelState.AddModelError(deleteAppointmentByIdResponse.Error);
        }
        
        return LocalRedirect(ReturnUrl);
    }
}