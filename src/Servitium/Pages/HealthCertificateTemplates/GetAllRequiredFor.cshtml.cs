using Application.Features.Appointments.Queries;
using Application.Features.Clients.Queries;
using Application.Features.HealthCertificateTemplates.Queries;
using Application.Features.HealthCertificateTemplates.Responces;
using Application.Features.HealthСertificatates.Queries;
using Application.Features.Persons.Queries;
using Application.Features.Services.Queries;
using Domain.Entities.Services;
using Infrastructure.Authentification;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Servitium.Extensions;

namespace Servitium.Pages.HealthCertificateTemplates;

public class GetAllRequiredFor(ISender sender) : PageModel
{
    [BindProperty] public int SelectedServiceId { get; set; }

    public List<(HealthCertificateTemplate Template, SelectList Services)> TemplatesWithCorrespondingServices
    {
        get;
        set;
    } = [];

    public List<(HealthCertificate Certificate, HealthCertificateTemplate Template)> CertificatesWithTheirTemplates
    {
        get;
        set;
    } = [];

    public List<(Appointment Appointment, Service Service)> AppointmentsWithTheirServices { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var userId = User.GetUserId();

        var getPersonByUserIdQuery = new GetPersonByUserIdQuery(userId);

        var getPersonByUserIdQueryResponse = await sender.Send(getPersonByUserIdQuery);

        if (getPersonByUserIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getPersonByUserIdQueryResponse.Error);
            return LocalRedirect(Routes.Index);
        }

        var person = getPersonByUserIdQueryResponse.Value;

        var getClientByPersonIdQuery = new GetClientByPersonIdQuery(person.Id ?? 0);

        var getClientByPersonIdQueryResponse = await sender.Send(getClientByPersonIdQuery);

        if (getClientByPersonIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getClientByPersonIdQueryResponse.Error);
            return RedirectToPage(Routes.Index);
        }

        var client = getClientByPersonIdQueryResponse.Value;

        var getNeededHealthCertificateTemplatesByMainHealthCertificateTemplateIdAndClientIdQuery =
            new GetNeededHealthCertificateTemplatesByMainHealthCertificateTemplateIdAndClientIdQuery(id,
                client.Id ?? 0);

        var getNeededHealthCertificateTemplatesByMainHealthCertificateTemplateIdAndClientIdQueryResponse =
            await sender.Send(getNeededHealthCertificateTemplatesByMainHealthCertificateTemplateIdAndClientIdQuery);

        if (getNeededHealthCertificateTemplatesByMainHealthCertificateTemplateIdAndClientIdQueryResponse.IsError)
        {
            ModelState.AddModelError(
                getNeededHealthCertificateTemplatesByMainHealthCertificateTemplateIdAndClientIdQueryResponse.Error);
            return RedirectToPage(Routes.Index);
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
                        ModelState.AddModelError(getAppointmentByIdQueryResponse.Error);
                        return LocalRedirect(Routes.Index);
                    }

                    var appointment = getAppointmentByIdQueryResponse.Value;

                    var getServiceByIdQuery = new GetServiceByIdQuery(appointment.ServiceId);

                    var getServiceByIdQueryResponse = await sender.Send(getServiceByIdQuery);

                    if (getServiceByIdQueryResponse.IsError)
                    {
                        ModelState.AddModelError(getServiceByIdQueryResponse.Error);
                        return LocalRedirect(Routes.Index);
                    }

                    var service = getServiceByIdQueryResponse.Value;

                    AppointmentsWithTheirServices.Add((appointment, service));

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
                        ModelState.AddModelError(getHealthCertificateTemplateByIdQueryResponse.Error);
                        return RedirectToPage(Routes.Index);
                    }

                    var template = getHealthCertificateTemplateByIdQueryResponse.Value;

                    var getServicesByResultTemplateIdQuery = new GetServicesByResultTemplateIdQuery(template.Id ?? 0);

                    var getServicesByResultTemplateIdQueryResponse =
                        await sender.Send(getServicesByResultTemplateIdQuery);

                    if (getServicesByResultTemplateIdQueryResponse.IsError)
                    {
                        ModelState.AddModelError(getServicesByResultTemplateIdQueryResponse.Error);
                        return RedirectToPage(Routes.Index);
                    }

                    var services = getServicesByResultTemplateIdQueryResponse.Value;

                    var selectList = new SelectList(services, "Id", "Name");

                    TemplatesWithCorrespondingServices.Add((template, selectList));

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
                        ModelState.AddModelError(
                            getLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryResponse.Error);
                        return RedirectToPage(Routes.Index);
                    }

                    var certificate = getLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryResponse
                        .Value;

                    var getHealthCertificateTemplateByIdQuery =
                        new GetHealthCertificateTemplateByIdQuery(certificate.TemplateId);

                    var getHealthCertificateTemplateByIdQueryResponse =
                        await sender.Send(getHealthCertificateTemplateByIdQuery);

                    if (getHealthCertificateTemplateByIdQueryResponse.IsError)
                    {
                        ModelState.AddModelError(getHealthCertificateTemplateByIdQueryResponse.Error);
                        return RedirectToPage(Routes.Index);
                    }

                    var template = getHealthCertificateTemplateByIdQueryResponse.Value;

                    CertificatesWithTheirTemplates.Add((certificate, template));

                    break;
                }
                default:
                    return RedirectToPage(Routes.Index);
            }
        }

        return Page();
    }

    public IActionResult OnPostAsync()
    {
        if (SelectedServiceId == 0)
        {
            return RedirectToPage(Routes.Index);
        }
        
        return RedirectToPage(Routes.AppointmentClientCreate, new {serviceId = SelectedServiceId});
    }
}