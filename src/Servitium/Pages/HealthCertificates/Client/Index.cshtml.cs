using Application.Features.Clients.Queries;
using Application.Features.HealthCertificateTemplates.Queries;
using Application.Features.HealthСertificatates.Queries;
using Application.Features.Persons.Queries;
using Domain.Entities.Services;
using Infrastructure.Authentification;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Servitium.Extensions;

namespace Servitium.Pages.HealthCertificates.Client;

public class Index(ISender sender) : PageModel
{
    public ICollection<(HealthCertificate HealthCertificate, HealthCertificateTemplate HealthCertificateTemplate)> Data
    {
        get;
        set;
    } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = User.GetUserId();

        var getPersonByUserIdQuery = new GetPersonByUserIdQuery(userId);

        var getPersonByUserIdQueryResponse = await sender.Send(getPersonByUserIdQuery);

        if (getPersonByUserIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getPersonByUserIdQueryResponse.Error);

            return RedirectToPage(Routes.Index);
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

        var getHealthCertificatesByClientIdQuery = new GetHealthCertificatesByClientIdQuery(client.Id ?? 0);

        var getHealthCertificatesByClientIdQueryResponse = await sender.Send(getHealthCertificatesByClientIdQuery);

        if (getHealthCertificatesByClientIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getHealthCertificatesByClientIdQueryResponse.Error);

            return RedirectToPage(Routes.Index);
        }

        var healthCertificates = getHealthCertificatesByClientIdQueryResponse.Value;

        List<(HealthCertificate HealthCertificate, HealthCertificateTemplate HealthCertificateTemplate)> data = [];

        foreach (var healthCertificate in healthCertificates)
        {
            var getHealthCertificateTemplateByIdQuery =
                new GetHealthCertificateTemplateByIdQuery(healthCertificate.TemplateId);

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