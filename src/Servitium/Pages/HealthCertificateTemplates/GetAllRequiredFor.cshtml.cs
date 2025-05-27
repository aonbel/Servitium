using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using Application.Features.Appointments.Queries;
using Application.Features.Clients.Queries;
using Application.Features.HealthCertificateTemplates.Queries;
using Application.Features.HealthCertificateTemplates.Responces;
using Application.Features.HealthСertificatates.Queries;
using Application.Features.Services.Queries;
using Domain.Abstractions.Result;
using Infrastructure.Authentification;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Servitium.Extensions;
using Servitium.Infrastructure.PagesConstants;

namespace Servitium.Pages.HealthCertificateTemplates;

public class GetAllRequiredFor(ISender sender) : PageModel
{
    public string ReturnUrl { get; set; } = Routes.AppointmentsClientIndex;

    public class DataModel
    {
        public List<string> AlreadyExistedHealthCertificateTemplateNames { get; set; } = [];
        public List<DateOnly> AlreadyExistedHealthCertificateReceivingTimes { get; set; } = [];
        public List<int> AlreadyExistedHealthCertificateIds { get; set; } = [];

        public List<string> AlreadyExistedAppointmentServiceShortNames { get; set; } = [];
        public List<DateOnly> AlreadyExistedAppointmentDates { get; set; } = [];
        public List<int> AlreadyExistedAppointmentIds { get; set; } = [];

        public List<string> NeededHealthCertificateTemplateNames { get; set; } = [];
        public List<SelectList> NeededHealthCertificateCorrespondingServices { get; set; } = [];
    }

    public class InputModel
    {
        [Required]
        public int SelectedServiceId { get; set; }
    }

    public DataModel Data { get; set; } = new();

    [BindProperty] public InputModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id, string? returnUrl = null)
    {
        if (returnUrl is not null)
        {
            ReturnUrl = returnUrl;
        }

        var loadingResult = await LoadDataAsync(id);

        if (loadingResult.IsError)
        {
            ModelState.AddModelError(loadingResult.Error);
            return LocalRedirect(ReturnUrl);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id, string? returnUrl = null)
    {
        if (returnUrl is not null)
        {
            ReturnUrl = returnUrl;
        }
        
        var userId = User.GetUserId();

        var getClientByUserIdQuery = new GetClientByUserIdQuery(userId);

        var getClientByUserIdQueryResponse = await sender.Send(getClientByUserIdQuery);

        if (getClientByUserIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getClientByUserIdQueryResponse.Error);

            return LocalRedirect(ReturnUrl);
        }

        var client = getClientByUserIdQueryResponse.Value;

        var checkIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQuery =
            new CheckIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQuery(client.Id ?? 0,
                Input.SelectedServiceId);

        var checkIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQueryResponse =
            await sender.Send(checkIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQuery);

        if (checkIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQueryResponse.IsError)
        {
            ModelState.AddModelError(checkIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQueryResponse
                .Error);
            return LocalRedirect(ReturnUrl);
        }

        var canCreate = checkIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQueryResponse.Value
            .CanCreate;

        if (!canCreate)
        {
            ModelState.AddModelError("", ErrorMessages.CannotCreateAppointmentBecauseOfDependencies);

            var loadingResult = await LoadDataAsync(id);

            if (loadingResult.IsError)
            {
                ModelState.AddModelError(loadingResult.Error);

                return LocalRedirect(ReturnUrl);
            }

            return Page();
        }

        return RedirectToPage(Routes.AppointmentClientCreate, new { serviceId = Input.SelectedServiceId });
    }

    public async Task<Result> LoadDataAsync(int healthCertificateTemplateId)
    {
        var userId = User.GetUserId();

        var getClientByUserIdQuery = new GetClientByUserIdQuery(userId);

        var getClientByUserIdQueryResponse = await sender.Send(getClientByUserIdQuery);

        if (getClientByUserIdQueryResponse.IsError)
        {
            return getClientByUserIdQueryResponse.Error;
        }

        var client = getClientByUserIdQueryResponse.Value;

        var getNeededHealthCertificateTemplatesByMainHealthCertificateTemplateIdAndClientIdQuery =
            new GetNeededHealthCertificateTemplatesByHealthCertificateTemplateIdAndClientIdQuery(
                healthCertificateTemplateId,
                client.Id ?? 0);

        var getNeededHealthCertificateTemplatesByMainHealthCertificateTemplateIdAndClientIdQueryResponse =
            await sender.Send(getNeededHealthCertificateTemplatesByMainHealthCertificateTemplateIdAndClientIdQuery);

        if (getNeededHealthCertificateTemplatesByMainHealthCertificateTemplateIdAndClientIdQueryResponse.IsError)
        {
            return getNeededHealthCertificateTemplatesByMainHealthCertificateTemplateIdAndClientIdQueryResponse.Error;
        }

        var requirements =
            getNeededHealthCertificateTemplatesByMainHealthCertificateTemplateIdAndClientIdQueryResponse
                .Value.Requirements;

        foreach (var requirement in requirements)
        {
            switch (requirement.Type)
            {
                case TypeOfRequirement.AppointmentId:
                {
                    var getAppointmentByIdQuery = new GetAppointmentByIdQuery(requirement.Id);
                    var getAppointmentByIdQueryResponse = await sender.Send(getAppointmentByIdQuery);

                    if (getAppointmentByIdQueryResponse.IsError)
                    {
                        return getAppointmentByIdQueryResponse.Error;
                    }

                    var appointment = getAppointmentByIdQueryResponse.Value;

                    var getServiceByIdQuery = new GetServiceByIdQuery(appointment.ServiceId);

                    var getServiceByIdQueryResponse = await sender.Send(getServiceByIdQuery);

                    if (getServiceByIdQueryResponse.IsError)
                    {
                        return getServiceByIdQueryResponse.Error;
                    }

                    var service = getServiceByIdQueryResponse.Value;

                    Data.AlreadyExistedAppointmentServiceShortNames.Add(service.ShortName);
                    Data.AlreadyExistedAppointmentDates.Add(appointment.Date);
                    Data.AlreadyExistedAppointmentIds.Add(appointment.Id ?? 0);

                    break;
                }
                case TypeOfRequirement.HealthCertificateTemplateId:
                {
                    var getHealthCertificateTemplateByIdQuery =
                        new GetHealthCertificateTemplateByIdQuery(requirement.Id);
                    var getHealthCertificateTemplateByIdQueryResponse =
                        await sender.Send(getHealthCertificateTemplateByIdQuery);

                    if (getHealthCertificateTemplateByIdQueryResponse.IsError)
                    {
                        return getHealthCertificateTemplateByIdQueryResponse.Error;
                    }

                    var template = getHealthCertificateTemplateByIdQueryResponse.Value;

                    var getServicesByResultTemplateIdQuery = new GetServicesByResultTemplateIdQuery(template.Id ?? 0);

                    var getServicesByResultTemplateIdQueryResponse =
                        await sender.Send(getServicesByResultTemplateIdQuery);

                    if (getServicesByResultTemplateIdQueryResponse.IsError)
                    {
                        return getServicesByResultTemplateIdQueryResponse.Error;
                    }

                    var services = getServicesByResultTemplateIdQueryResponse.Value;

                    var selectList = new SelectList(services, "Id", "Name");

                    Data.NeededHealthCertificateTemplateNames.Add(template.Name);
                    Data.NeededHealthCertificateCorrespondingServices.Add(selectList);

                    break;
                }
                case TypeOfRequirement.HealthCertificateId:
                {
                    var getLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQuery =
                        new GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQuery(
                            client.Id ?? 0,
                            requirement.Id);

                    var getLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryResponse =
                        await sender.Send(getLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQuery);

                    if (getLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryResponse.IsError)
                    {
                        return getLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryResponse.Error;
                    }

                    var certificate = getLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryResponse
                        .Value.HealthCertificate;

                    if (certificate is null)
                    {
                        return new Error("", "No health certificate was found");
                    }

                    var getHealthCertificateTemplateByIdQuery =
                        new GetHealthCertificateTemplateByIdQuery(certificate.TemplateId);

                    var getHealthCertificateTemplateByIdQueryResponse =
                        await sender.Send(getHealthCertificateTemplateByIdQuery);

                    if (getHealthCertificateTemplateByIdQueryResponse.IsError)
                    {
                        return getHealthCertificateTemplateByIdQueryResponse.Error;
                    }

                    var template = getHealthCertificateTemplateByIdQueryResponse.Value;

                    Data.AlreadyExistedHealthCertificateTemplateNames.Add(template.Name);
                    Data.AlreadyExistedHealthCertificateReceivingTimes.Add(certificate.ReceivingTime);
                    Data.AlreadyExistedHealthCertificateIds.Add(certificate.Id ?? 0);

                    break;
                }
            }
        }

        return Result.Success();
    }
}