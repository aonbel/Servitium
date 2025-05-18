using Application.Features.HealthCertificateTemplates.Queries;
using Domain.Entities.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Servitium.Pages.HealthCertificateTemplates.Admin;

public class Index(ISender sender) : PageModel
{
    public ICollection<HealthCertificateTemplate> Data { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var getAllHealthCertificateTemplates = new GetAllHealthCertificateTemplates();

        var getAllHealthCertificateTemplatesResponse = await sender.Send(getAllHealthCertificateTemplates);

        if (getAllHealthCertificateTemplatesResponse.IsError)
        {
            ModelState.AddModelError(getAllHealthCertificateTemplatesResponse.Error.Code, getAllHealthCertificateTemplatesResponse.Error.Message);
            return Page();
        }
        
        var healthCertificateTemplates = getAllHealthCertificateTemplatesResponse.Value;
        
        Data = healthCertificateTemplates;
        
        return Page();
    }
}