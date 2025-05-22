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

namespace Servitium.Pages.Appointments.Client;

public class Create(ISender sender) : PageModel
{
    public const int DaysOffset = 7;

    public int ServiceId { get; set; }

    public List<SelectListItem> DaysSelectList { get; set; } = [];

    [BindProperty] public InputModel Input { get; set; } = new();

    public class InputModel
    {
        public DateOnly SelectedDate { get; set; }

        public int SelectedServiceProviderId { get; set; }

        public int SelectedSpecialistId { get; set; }

        public string SelectedTimeOnlySegment { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGet(int serviceId, string? returnUrl = null)
    {
        ServiceId = serviceId;
        
        returnUrl ??= Routes.AppointmentsClientIndex;

        var userId = User.GetUserId();

        var getPersonByUserIdQuery = new GetPersonByUserIdQuery(userId);

        var getPersonByUserIdQueryResponse = await sender.Send(getPersonByUserIdQuery);

        if (getPersonByUserIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getPersonByUserIdQueryResponse.Error);
            return LocalRedirect(returnUrl);
        }

        var person = getPersonByUserIdQueryResponse.Value;

        var getClientByPersonIdQuery = new GetClientByPersonIdQuery(person.Id ?? 0);

        var getClientByPersonIdQueryResponse = await sender.Send(getClientByPersonIdQuery);

        if (getClientByPersonIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getClientByPersonIdQueryResponse.Error);
            return LocalRedirect(returnUrl);
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
            return LocalRedirect(returnUrl);
        }

        var result = checkIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQueryResponse.Value;

        if (!result.CanCreate)
        {
            ModelState.AddModelError("CantCreateAppointment", "Can't create appointment");
            return LocalRedirect(returnUrl);
        }

        var fromDate = result.MinDateTime!.Value;

        for (var dayOffset = 0; dayOffset < DaysOffset; dayOffset++)
        {
            var currentDate = fromDate.AddDays(dayOffset);

            DaysSelectList.Add(new SelectListItem(currentDate.ToString("MM/dd/yyyy"),
                currentDate.ToString("MM/dd/yyyy")));
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int serviceId, string? returnUrl = null)
    {
        returnUrl ??= Url.Content(Routes.AppointmentsClientIndex);

        var userId = User.GetUserId();

        var getPersonByUserIdQuery = new GetPersonByUserIdQuery(userId);

        var getPersonByUserIdQueryResponse = await sender.Send(getPersonByUserIdQuery);

        if (getPersonByUserIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getPersonByUserIdQueryResponse.Error);
            return RedirectToPage(Routes.AppointmentsClientIndex);
        }

        var person = getPersonByUserIdQueryResponse.Value;

        var getClientByPersonIdQuery = new GetClientByPersonIdQuery(person.Id ?? 0);

        var getClientByPersonIdQueryResponse = await sender.Send(getClientByPersonIdQuery);

        if (getClientByPersonIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getClientByPersonIdQueryResponse.Error);
            return RedirectToPage(Routes.AppointmentsClientIndex);
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
            return RedirectToPage(Routes.AppointmentsClientIndex);
        }

        return LocalRedirect(returnUrl);
    }
}