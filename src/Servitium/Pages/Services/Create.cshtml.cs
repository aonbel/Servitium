using System.ComponentModel.DataAnnotations;
using Application.Features.HealthCertificateTemplates.Queries;
using Application.Features.Services.Commands;
using Domain.Abstractions.Result;
using Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Servitium.Extensions;
using Servitium.Infrastructure.PagesConstants;

namespace Servitium.Pages.Services;

[Authorize(Roles = ApplicationRoles.AdminOrManager)]
public class Create(ISender sender) : PageModel
{
    public string ReturnUrl { get; set; } = Routes.ServicesAdminIndex;

    public DataModel Data { get; set; } = new();

    [BindProperty] public InputModel Input { get; set; } = new();

    public class DataModel
    {
        public List<SelectListItem> HealthCertificateTemplates { get; set; } = [];
    }

    public class InputModel
    {
        [Required] [DataType(DataType.Text)] public string Name { get; set; } = string.Empty;

        [Required] [DataType(DataType.Text)] public string ShortName { get; set; } = string.Empty;

        [Required] [DataType(DataType.Text)] public string Description { get; set; } = string.Empty;

        public ICollection<int> RequiredHealthCertificateTemplateIds { get; set; } = [];

        [Required] public ICollection<int> ResultHealthCertificateTemplateIds { get; set; } = [];

        [Required]
        [DataType(DataType.Currency)]
        public decimal PricePerHourForMaterials { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal PricePerHourForEquipment { get; set; }

        [Required]
        [DataType(DataType.Duration)]
        [DisplayFormat(DataFormatString = @"{0:hh\:mm}", ApplyFormatInEditMode = true)]
        [Range(typeof(TimeSpan), Constraints.MinServiceDuration, Constraints.MaxServiceDuration,
            ErrorMessage = ErrorMessages.ServiceDuration)]
        public TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(1);
    }

    public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
    {
        if (returnUrl is not null)
        {
            ReturnUrl = returnUrl;
        }

        var result = await LoadHealthCertificateTemplates();

        if (result.IsError)
        {
            ModelState.AddModelError(result.Error);
        }
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        if (returnUrl is not null)
        {
            ReturnUrl = returnUrl;
        }
        
        if (!ModelState.IsValid)
        {
            var result = await LoadHealthCertificateTemplates();

            if (result.IsError)
            {
                ModelState.AddModelError(result.Error);
            }
            
            return Page();
        }

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
        }

        return LocalRedirect(ReturnUrl);
    }
    
    public async Task<Result> LoadHealthCertificateTemplates()
    {
        var getAllHealthCertificateTemplates = new GetAllHealthCertificateTemplatesQuery();

        var getAllHealthCertificateTemplatesResponse = await sender.Send(getAllHealthCertificateTemplates);

        if (getAllHealthCertificateTemplatesResponse.IsError)
        {
            return getAllHealthCertificateTemplatesResponse;
        }

        var healthCertificateTemplates = getAllHealthCertificateTemplatesResponse.Value;

        Data = new DataModel
        {
            HealthCertificateTemplates = healthCertificateTemplates
                .Select(t => new SelectListItem(t.Name, t.Id.ToString())).ToList()
        };
        
        return Result.Success();
    }
}