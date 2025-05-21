using Application.Features.HealthCertificateTemplates.Queries;
using Application.Features.HealthСertificatates.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Servitium.Extensions;

namespace Servitium.Pages.HealthCertificates;

public class Details(ISender sender) : PageModel
{
    public string ReturnUrl { get; set; } = Routes.Index;
    
    public HealthCertificateDetails Data { get; set; } = new ();

    public class HealthCertificateDetails
    {
        public string TemplateName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
    
    public async Task<IActionResult> OnGetAsync(int id, string? returnUrl = null)
    {
        if (returnUrl is not null)
        {
            ReturnUrl = returnUrl;
        }
        
        var getHealthCertificateByIdQuery = new GetHealthCertificateByIdQuery(id);
        
        var getHealthCertificateByIdQueryResponse = await sender.Send(getHealthCertificateByIdQuery);

        if (getHealthCertificateByIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getHealthCertificateByIdQueryResponse.Error);
            return RedirectToPage(Routes.Index);
        }
        
        var certificate = getHealthCertificateByIdQueryResponse.Value;

        var getHealthCertificateTemplateByIdQuery = new GetHealthCertificateTemplateByIdQuery(certificate.TemplateId);
        
        var getHealthCertificateTemplateByIdQueryResponse = await sender.Send(getHealthCertificateTemplateByIdQuery);

        if (getHealthCertificateTemplateByIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getHealthCertificateTemplateByIdQueryResponse.Error);
            return RedirectToPage(Routes.Index);
        }
        
        var template = getHealthCertificateTemplateByIdQueryResponse.Value;
        
        Data = new HealthCertificateDetails()
        {
            TemplateName = template.Name,
            Description = certificate.Description
        };

        return Page();
    }
}