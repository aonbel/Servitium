using System.ComponentModel.DataAnnotations;
using Application.Features.HealthCertificateTemplates.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Servitium.Extensions;

namespace Servitium.Pages.HealthCertificates.Client;

public class Create(ISender sender) : PageModel
{
    public SelectList HealthCertificateSelectList { get; set; }

    [BindProperty] public InputModel Input { get; set; } = new();

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
            return LocalRedirect(Routes.Index);
        }

        var templates = getAllHealthCertificateTemplatesQueryResponse.Value;

        HealthCertificateSelectList = new SelectList(templates, "Id", "Name");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        return RedirectToPage(Routes.HealthCertificateTemplateGetAllRequiredFor, new { id = Input.TemplateId });
    }
}