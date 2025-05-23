using Application.Features.HealthCertificateTemplates.Queries;
using Application.Features.HealthСertificatates.Queries;
using Application.Features.Services.Queries;
using Domain.Entities.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Servitium.Extensions;

namespace Servitium.Pages.HealthCertificates.Specialist;

public class Index(ISender sender) : PageModel
{
    public ICollection<(HealthCertificate HealthCertificate, HealthCertificateTemplate HealthCertificateTemplate)> Data
    {
        get;
        set;
    } = [];

    public async Task<IActionResult> OnGetAsync(int serviceId, int clientId)
    {
        var getServiceByIdQuery = new GetServiceByIdQuery(serviceId);

        var getServiceByIdQueryResponse = await sender.Send(getServiceByIdQuery);

        if (getServiceByIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getServiceByIdQueryResponse.Error);
            return RedirectToPage(Routes.AppointmentSpecialistIndex);
        }

        var service = getServiceByIdQueryResponse.Value;

        List<(HealthCertificate HealthCertificate, HealthCertificateTemplate HealthCertificateTemplate)> data = [];

        foreach (var healthCertificateTemplateId in service.RequiredHealthCertificateTemplateIds)
        {
            var getLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQuery =
                new GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQuery(clientId,
                    healthCertificateTemplateId);

            var getLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryResponse =
                await sender.Send(getLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQuery);

            if (getLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryResponse.IsError)
            {
                ModelState.AddModelError(getLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryResponse
                    .Error);
                return RedirectToPage(Routes.AppointmentSpecialistIndex);
            }
            
            var healthCertificate = getLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryResponse.Value.HealthCertificate;

            if (healthCertificate is null)
            {
                ModelState.AddModelError("", "No health certificate was found");
                return RedirectToPage(Routes.AppointmentSpecialistIndex);
            }
            
            var getHealthCertificateTemplateByIdQuery =
                new GetHealthCertificateTemplateByIdQuery(healthCertificateTemplateId);

            var getHealthCertificateTemplateByIdQueryResponse =
                await sender.Send(getHealthCertificateTemplateByIdQuery);

            if (getHealthCertificateTemplateByIdQueryResponse.IsError)
            {
                ModelState.AddModelError(getHealthCertificateTemplateByIdQueryResponse.Error);

                return RedirectToPage(Routes.Index);
            }

            var healthCertificateTemplate = getHealthCertificateTemplateByIdQueryResponse.Value;
            
            data.Add((healthCertificate, healthCertificateTemplate));
        }

        Data = data;

        return Page();
    }
}