using System.ComponentModel.DataAnnotations;
using Application.Features.HealthCertificateTemplates.Queries;
using Application.Features.Services.Commands;
using Application.Features.Services.Queries;
using Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Servitium.Extensions;

namespace Servitium.Pages.Services;

[Authorize(Roles = ApplicationRoles.AdminOrManager)]
public class Create(ISender sender) : PageModel
{
    public SelectList HealthCertificateTemplates { get; set; }

    [BindProperty] public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required] [DataType(DataType.Text)] public string Name { get; set; } = string.Empty;

        [Required] [DataType(DataType.Text)] public string ShortName { get; set; } = string.Empty;

        [Required] [DataType(DataType.Text)] public string Description { get; set; } = string.Empty;

        [Required] public ICollection<int> RequiredHealthCertificateTemplateIds { get; set; } = [];

        [Required] public ICollection<int> ResultHealthCertificateTemplateIds { get; set; } = [];

        [Required]
        [DataType(DataType.Currency)]
        public decimal PricePerHourForMaterials { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal PricePerHourForEquipment { get; set; }

        [Required]
        [DataType(DataType.Duration)]
        public TimeSpan Duration { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var getAllHealthCertificateTemplates = new GetAllHealthCertificateTemplatesQuery();

        var getAllHealthCertificateTemplatesResponse = await sender.Send(getAllHealthCertificateTemplates);

        if (getAllHealthCertificateTemplatesResponse.IsError)
        {
            ModelState.AddModelError(getAllHealthCertificateTemplatesResponse.Error);
            return Page();
        }

        var healthCertificateTemplates = getAllHealthCertificateTemplatesResponse.Value;

        HealthCertificateTemplates = new SelectList(healthCertificateTemplates, "Id", "Name");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Routes.Index;

        var createServiceCommand = new CreateServiceCommand(
            Input.Name,
            Input.ShortName,
            Input.Description,
            Input.RequiredHealthCertificateTemplateIds,
            Input.ResultHealthCertificateTemplateIds,
            Input.PricePerHourForMaterials,
            Input.PricePerHourForEquipment,
            Input.Duration);
        
        var createServiceResponse = await sender.Send(createServiceCommand);

        if (createServiceResponse.IsError)
        {
            ModelState.AddModelError(createServiceResponse.Error);
            return Page();
        }
        
        return LocalRedirect(returnUrl);
    }
}