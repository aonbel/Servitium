using System.ComponentModel.DataAnnotations;
using Application.Features.Appointments.Queries;
using Application.Features.Clients.Queries;
using Application.Features.HealthCertificateTemplates.Queries;
using Application.Features.HealthСertificatates.Queries;
using Application.Features.Persons.Queries;
using Application.Features.Services.Queries;
using Domain.Entities.Core;
using Domain.Entities.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Servitium.Extensions;

namespace Servitium.Pages.Appointments.Specialist;

public class Details(ISender sender) : PageModel
{
    public string ReturnUrl { get; set; } = Routes.AppointmentSpecialistIndex;

    public DataModel Data { get; set; } = new();

    public class DataModel
    {
        [Display(Name = "Date of appointment")]
        public DateOnly AppointmentDate { get; set; }

        [Display(Name = "Time of appointment")]
        public TimeOnlySegment AppointmentTime { get; set; } = new();

        [Display(Name = "Service short name")] public string ServiceShortName { get; set; } = string.Empty;

        [Display(Name = "Client full name")] public string ClientFullName { get; set; } = string.Empty;

        public int ClientId { get; set; }

        public int ServiceId { get; set; }

        public List<int> ResultHealthCertificateTemplateIds { get; set; } = [];

        public List<string> ResultHealthCertificateTemplateNames { get; set; } = [];
    }

    public async Task<IActionResult> OnGetAsync(int appointmentId, string? returnUrl = null)
    {
        if (returnUrl is not null)
        {
            ReturnUrl = returnUrl;
        }

        var getAppointmentByIdQuery = new GetAppointmentByIdQuery(appointmentId);

        var getAppointmentByIdQueryResponse = await sender.Send(getAppointmentByIdQuery);

        if (getAppointmentByIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getAppointmentByIdQueryResponse.Error);
            return LocalRedirect(ReturnUrl);
        }

        var appointment = getAppointmentByIdQueryResponse.Value;

        var getServiceByIdQuery = new GetServiceByIdQuery(appointment.ServiceId);

        var getServiceByIdQueryResponse = await sender.Send(getServiceByIdQuery);

        if (getServiceByIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getServiceByIdQueryResponse.Error);
            return LocalRedirect(ReturnUrl);
        }

        var service = getServiceByIdQueryResponse.Value;

        var getClientByIdQuery = new GetClientByIdQuery(appointment.ClientId);

        var getClientByIdQueryResponse = await sender.Send(getClientByIdQuery);

        if (getClientByIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getClientByIdQueryResponse.Error);
            return LocalRedirect(ReturnUrl);
        }

        var client = getClientByIdQueryResponse.Value;

        var getPersonByIdQuery = new GetPersonByIdQuery(client.PersonId);

        var getPersonByIdQueryResponse = await sender.Send(getPersonByIdQuery);

        if (getPersonByIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getPersonByIdQueryResponse.Error);
            return LocalRedirect(ReturnUrl);
        }

        var clientPerson = getPersonByIdQueryResponse.Value;

        var getHealthCertificatesByClientIdQuery = new GetHealthCertificatesByClientIdQuery(client.Id ?? 0);

        var getHealthCertificatesByClientIdQueryResponse = await sender.Send(getHealthCertificatesByClientIdQuery);

        if (getHealthCertificatesByClientIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getHealthCertificatesByClientIdQueryResponse.Error);
            return LocalRedirect(ReturnUrl);
        }

        var clientHealthCertificates = getHealthCertificatesByClientIdQueryResponse.Value;

        var clientHealthCertificateTemplateIds = clientHealthCertificates.Select(c => c.TemplateId).ToList();

        List<HealthCertificateTemplate> resultHealthCertificateTemplates = [];

        foreach (var resultHealthCertificateTemplateId in service.ResultHealthCertificateTemplateIds)
        {
            if (clientHealthCertificateTemplateIds.Contains(resultHealthCertificateTemplateId))
            {
                continue;
            }

            var getHealthCertificateByIdQuery =
                new GetHealthCertificateTemplateByIdQuery(resultHealthCertificateTemplateId);

            var getHealthCertificateByIdQueryResponse = await sender.Send(getHealthCertificateByIdQuery);

            if (getAppointmentByIdQueryResponse.IsError)
            {
                ModelState.AddModelError(getAppointmentByIdQueryResponse.Error);
                return RedirectToPage(Routes.AppointmentSpecialistIndex);
            }

            var healthCertificateTemplate = getHealthCertificateByIdQueryResponse.Value;

            resultHealthCertificateTemplates.Add(healthCertificateTemplate);
        }

        Data = new DataModel
        {
            AppointmentDate = appointment.Date,
            AppointmentTime = appointment.TimeSegment,
            ServiceShortName = service.ShortName,
            ClientFullName = $"{clientPerson.FirstName} {clientPerson.LastName}",
            ClientId = client.Id ?? 0,
            ServiceId = service.Id ?? 0,
            ResultHealthCertificateTemplateIds = resultHealthCertificateTemplates.Select(t => t.Id ?? 0).ToList(),
            ResultHealthCertificateTemplateNames = resultHealthCertificateTemplates.Select(t => t.Name).ToList()
        };

        return Page();
    }
}