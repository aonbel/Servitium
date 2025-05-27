using System.ComponentModel.DataAnnotations;
using Application.Features.HealthCertificateTemplates.Queries;
using Application.Features.HealthСertificatates.Commands;
using Domain.Entities.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Servitium.Extensions;

namespace Servitium.Pages.HealthCertificates.Specialist;

public class Create(ISender sender) : PageModel
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
    
    public async Task<IActionResult> OnGetAsync(int healthCertificateTemplateId, int clientId, string? returnUrl = null)
    {
        if (returnUrl is not null)
        {
            ReturnUrl = returnUrl;
        }
        
        var getHealthCertificateTemplateByIdQuery = new GetHealthCertificateTemplateByIdQuery(healthCertificateTemplateId);
        
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

    public async Task<IActionResult> OnPostAsync(int healthCertificateTemplateId, int clientId)
    {
        var createHealthCertificateCommand = new CreateHealthCertificateCommand(healthCertificateTemplateId, clientId, Input.Description);
        
        var getHealthCertificateCommandResponse = await sender.Send(createHealthCertificateCommand);

        if (getHealthCertificateCommandResponse.IsError)
        {
            ModelState.AddModelError(getHealthCertificateCommandResponse.Error);
            return LocalRedirect(ReturnUrl);
        }
        
        return LocalRedirect(ReturnUrl);
    }
}