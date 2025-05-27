using System.ComponentModel.DataAnnotations;
using Application.Features.Appointments.Commands;
using Application.Features.Appointments.Queries;
using Application.Features.Clients.Queries;
using Application.Features.Persons.Queries;
using Domain.Entities.Core;
using Infrastructure.Authentification;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Servitium.Extensions;
using Servitium.Infrastructure.PagesConstants;

namespace Servitium.Pages.Appointments.Client;

public class Create(ISender sender) : PageModel
{
    public string ReturnUrl { get; set; } = Routes.AppointmentsClientIndex;
    
    public class DataModel
    {
        public int ServiceId { get; set; }
        
        public string DateFrom { get; set; } = string.Empty;
        
        public string TimeFrom { get; set; } = string.Empty;
        
        public List<SelectListItem> DaysSelectList { get; set; } = [];
    }
    
    public class InputModel
    {
        [Required]
        [Display(Name = "Date")]
        public DateOnly SelectedDate { get; set; }
        
        [Required]
        [Display(Name = "Service provider")]
        public int SelectedServiceProviderId { get; set; }
        
        [Required]
        [Display(Name = "Specialist")]
        public int SelectedSpecialistId { get; set; }
        
        [Required]
        [Display(Name = "Time")]
        public string SelectedTimeOnlySegment { get; set; } = string.Empty;
    }
    
    public DataModel Data { get; set; } = new ();
    
    [BindProperty] public InputModel Input { get; set; } = new();

    public async Task<IActionResult> OnGet(int serviceId, string? returnUrl = null)
    {
        if (returnUrl is not null)
        {
            ReturnUrl = returnUrl;
        }
        
        Data.ServiceId = serviceId;

        var userId = User.GetUserId();

        var getPersonByUserIdQuery = new GetPersonByUserIdQuery(userId);

        var getPersonByUserIdQueryResponse = await sender.Send(getPersonByUserIdQuery);

        if (getPersonByUserIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getPersonByUserIdQueryResponse.Error);
            return LocalRedirect(ReturnUrl);
        }

        var person = getPersonByUserIdQueryResponse.Value;

        var getClientByPersonIdQuery = new GetClientByPersonIdQuery(person.Id ?? 0);

        var getClientByPersonIdQueryResponse = await sender.Send(getClientByPersonIdQuery);

        if (getClientByPersonIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getClientByPersonIdQueryResponse.Error);
            return LocalRedirect(ReturnUrl);
        }

        var client = getClientByPersonIdQueryResponse.Value;

        var checkIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQuery =
            new CheckIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQuery(
                client.Id ?? 0,
                serviceId);

        var checkIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQueryResponse =
            await sender.Send(checkIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQuery);

        if (checkIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQueryResponse.IsError)
        {
            ModelState.AddModelError(
                checkIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQueryResponse.Error);
            return LocalRedirect(ReturnUrl);
        }

        var result = checkIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQueryResponse.Value;

        if (!result.CanCreate)
        {
            ModelState.AddModelError("CantCreateAppointment", "Can't create appointment");
            return LocalRedirect(ReturnUrl);
        }

        var fromDate = DateOnly.FromDateTime(result.MinDateTime!.Value);
        var fromTime = TimeOnly.FromDateTime(result.MinDateTime!.Value);
        
        for (var dayOffset = 0; dayOffset < DaysOfWeek.DaysOfWeekCount; dayOffset++)
        {
            var currentDate = fromDate.AddDays(dayOffset);

            Data.DaysSelectList.Add(new SelectListItem(currentDate.ToString("MM/dd/yyyy"),
                currentDate.ToString("MM/dd/yyyy")));
        }
        
        Data.DateFrom = fromDate.ToString("O");
        Data.TimeFrom = fromTime.ToString("O");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int serviceId, string? returnUrl = null)
    {
        if (returnUrl is not null)
        {
            ReturnUrl = returnUrl;
        }

        var userId = User.GetUserId();

        var getPersonByUserIdQuery = new GetPersonByUserIdQuery(userId);

        var getPersonByUserIdQueryResponse = await sender.Send(getPersonByUserIdQuery);

        if (getPersonByUserIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getPersonByUserIdQueryResponse.Error);
            return RedirectToPage(ReturnUrl);
        }

        var person = getPersonByUserIdQueryResponse.Value;

        var getClientByPersonIdQuery = new GetClientByPersonIdQuery(person.Id ?? 0);

        var getClientByPersonIdQueryResponse = await sender.Send(getClientByPersonIdQuery);

        if (getClientByPersonIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getClientByPersonIdQueryResponse.Error);
            return RedirectToPage(ReturnUrl);
        }

        var client = getClientByPersonIdQueryResponse.Value;

        var times = Input.SelectedTimeOnlySegment.Split('-');

        var timeOnlySegmentBegin = TimeOnly.Parse(times[0]);
        var timeOnlySegmentEnd = TimeOnly.Parse(times[1]);

        var createAppointmentCommand = new CreateAppointmentCommand(
            serviceId,
            client.Id ?? 0,
            Input.SelectedSpecialistId,
            Input.SelectedDate,
            new TimeOnlySegment(timeOnlySegmentBegin, timeOnlySegmentEnd));

        var createAppointmentResponse = await sender.Send(createAppointmentCommand);

        if (createAppointmentResponse.IsError)
        {
            ModelState.AddModelError(createAppointmentResponse.Error);
            return RedirectToPage(ReturnUrl);
        }

        return LocalRedirect(ReturnUrl);
    }
}