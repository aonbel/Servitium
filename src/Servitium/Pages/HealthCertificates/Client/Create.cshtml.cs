using System.ComponentModel.DataAnnotations;
using Application.Features.HealthCertificateTemplates.Queries;
using Domain.Abstractions.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Servitium.Extensions;

namespace Servitium.Pages.HealthCertificates.Client;

public class Create(ISender sender, ILogger<Create> logger) : PageModel
{
    public string ReturnUrl { get; set; } = Routes.HealthCertificatesClientIndex;

    public DataModel Data { get; set; } = new();

    [BindProperty] public InputModel Input { get; set; } = new();

    public class DataModel
    {
        [Required] public List<SelectListItem> HealthCertificateSelectList { get; set; } = [];
    }

    public class InputModel
    {
        [Required] public int TemplateId { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var getAllHealthCertificateTemplatesQuery = new GetAllHealthCertificateTemplatesQuery();

        var getAllHealthCertificateTemplatesQueryResponse = await sender.Send(getAllHealthCertificateTemplatesQuery);

        if (getAllHealthCertificateTemplatesQueryResponse.IsError)
        {
            ModelState.AddModelError(getAllHealthCertificateTemplatesQueryResponse.Error);
            return LocalRedirect(ReturnUrl);
        }

        var templates = getAllHealthCertificateTemplatesQueryResponse.Value;
        
        logger.LogInformation("Return url {returnUrl}", ReturnUrl);
        
        foreach (var template in templates)
        {
            logger.LogInformation("Observing template {templateId}", template.Id);
            
            var getNeededHealthCertificateTemplatesByHealthCertificateTemplateIdQuery =
                new GetNeededHealthCertificateTemplatesByHealthCertificateTemplateIdQuery(template.Id ?? 0);

            var getNeededHealthCertificateTemplatesByHealthCertificateTemplateIdQueryResponse =
                await sender.Send(getNeededHealthCertificateTemplatesByHealthCertificateTemplateIdQuery);

            if (getNeededHealthCertificateTemplatesByHealthCertificateTemplateIdQueryResponse.IsError)
            {
                ModelState.AddModelError(getNeededHealthCertificateTemplatesByHealthCertificateTemplateIdQueryResponse
                    .Error);
                return LocalRedirect(ReturnUrl);
            }

            var getNeededHealthCertificateTemplatesByHealthCertificateTemplateIdQueryResult =
                getNeededHealthCertificateTemplatesByHealthCertificateTemplateIdQueryResponse.Value.Result;
            
            logger.LogInformation("Add template {templateName}", template.Name);

            Data.HealthCertificateSelectList.Add(new SelectListItem(
                    template.Name,
                    template.Id.ToString(),
                    false,
                    getNeededHealthCertificateTemplatesByHealthCertificateTemplateIdQueryResult is null));
        }
        
        return Page();
    }

    public IActionResult OnPost()
    {
        return RedirectToPage(Routes.HealthCertificateTemplateGetAllRequiredFor, new { id = Input.TemplateId });
    }
}