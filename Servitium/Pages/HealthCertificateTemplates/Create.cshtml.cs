using System.ComponentModel.DataAnnotations;
using Application.Features.HealthCertificateTemplates.Commands;
using Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Servitium.Extensions;

namespace Servitium.Pages.HealthCertificateTemplates;

[Authorize(Roles = ApplicationRoles.Admin)]
public class Create(ISender sender) : PageModel
{
    [BindProperty] public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required] [DataType(DataType.Text)] public string Name { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Duration)]
        public TimeSpan ActivePeriod { get; set; } = TimeSpan.Zero;
    };

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content(Routes.Index);
        
        var createHealthCertificateTemplateCommand =
            new CreateHealthCertificateTemplateCommand(Input.Name, Input.ActivePeriod);

        var createHealthCertificateTemplateCommandResponse = await sender.Send(createHealthCertificateTemplateCommand);

        if (createHealthCertificateTemplateCommandResponse.IsError)
        {
            ModelState.AddModelError(createHealthCertificateTemplateCommandResponse.Error);
            return Page();
        }
        
        return LocalRedirect(returnUrl);
    }
}