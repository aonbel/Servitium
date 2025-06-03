using System.ComponentModel.DataAnnotations;
using Application.Features.HealthCertificateTemplates.Queries;
using Application.Features.HealthСertificatates.Commands;
using Application.Features.HealthСertificatates.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Servitium.Extensions;

namespace Servitium.Pages.HealthCertificates.Specialist;

public class Update(ISender sender) : PageModel
{
    public string ReturnUrl { get; private set; } = Routes.AppointmentSpecialistIndex;

    public DataModel Data { get; set; } = new();

    [BindProperty] public InputModel Input { get; set; } = new();
    
    public class InputModel
    {
        [Display(Name = "Description for health certificate")]
        public string Description { get; set; } = string.Empty;
    }

    public class DataModel
    {
        public string HealthCertificateTemplateName { get; set; } = string.Empty;
    }
    
    public async Task<IActionResult> OnGetAsync(int id, string? returnUrl = null)
    {
        if (returnUrl is not null)
        {
            ReturnUrl = returnUrl;
        }

        var getHealthCertificateByIdQuery = new GetHealthCertificateByIdQuery(id);
        
        var getHealthCertificateByIdQueryResponce = await sender.Send(getHealthCertificateByIdQuery);

        if (getHealthCertificateByIdQueryResponce.IsError)
        {
            ModelState.AddModelError(getHealthCertificateByIdQueryResponce.Error);
            return LocalRedirect(ReturnUrl);
        }
        
        var healthCertificate = getHealthCertificateByIdQueryResponce.Value;
        
        var getHealthCertificateTemplateByIdQuery = new GetHealthCertificateTemplateByIdQuery(healthCertificate.TemplateId);
        
        var getHealthCertificateTemplateByIdQueryResponse = await sender.Send(getHealthCertificateTemplateByIdQuery);

        if (getHealthCertificateTemplateByIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getHealthCertificateTemplateByIdQueryResponse.Error);
            return LocalRedirect(ReturnUrl);
        }
        
        var healthCertificateTemplate = getHealthCertificateTemplateByIdQueryResponse.Value;
        
        Data.HealthCertificateTemplateName = healthCertificateTemplate.Name;
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id, string? returnUrl = null)
    {
        if (returnUrl is not null)
        {
            ReturnUrl = returnUrl;
        }
        
        var updateHealthCertificateStatusCommand = new UpdateHealthCertificateStatusCommand(id, Input.Description);
        
        var updateHealthCertificateStatusCommandResponse = await sender.Send(updateHealthCertificateStatusCommand);

        if (updateHealthCertificateStatusCommandResponse.IsError)
        {
            ModelState.AddModelError(updateHealthCertificateStatusCommandResponse.Error);
            return LocalRedirect(ReturnUrl);
        }
        
        return LocalRedirect(ReturnUrl);
    }
}