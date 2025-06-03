using System.ComponentModel.DataAnnotations;
using Application.Features.ServiceProviders.Commands;
using Domain.Entities.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Servitium.Extensions;
using Servitium.Infrastructure.PagesConstants;

namespace Servitium.Pages.ServiceProviders;

public class Create(ISender sender) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        [StringLength(Constraints.MaxServiceProviderName, ErrorMessage = ErrorMessages.StringLengthRequirements,
            MinimumLength = Constraints.MinServiceProviderName)]
        [Display(Name = "Service provider name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(Constraints.MaxServiceProviderShortName, ErrorMessage = ErrorMessages.StringLengthRequirements,
            MinimumLength = Constraints.MinServiceProviderShortName)]
        [Display(Name = "Service provider short name")]
        public string ShortName { get; set; } = string.Empty;

        [Required]
        [StringLength(Constraints.MaxServiceProviderAddress, ErrorMessage = ErrorMessages.StringLengthRequirements,
            MinimumLength = Constraints.MinServiceProviderAddress)]
        [Display(Name = "Service provider address")]
        public string Address { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Work start time in utc")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public TimeOnly WorkBeginTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Work end time in utc")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public TimeOnly WorkEndTime { get; set; }

        [Required(ErrorMessage = "Select at least one day")]
        [Display(Name = "Work days")]
        public List<DayOfWeek> SelectedWorkDays { get; set; } = [];

        [Display(Name = "Contacts")] public string Contacts { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var command = new CreateServiceProviderCommand(Input.Name, Input.ShortName, Input.Address,
            new TimeOnlySegment(Input.WorkBeginTime.ToTimeSpan(), Input.WorkEndTime.ToTimeSpan()),
            Input.SelectedWorkDays,
            Input.Contacts.Split(',')
        );

        var responce = await sender.Send(command);

        if (responce.IsError)
        {
            ModelState.AddModelError(responce.Error.Code, responce.Error.Message);
            return LocalRedirect(Routes.Index);
        }

        return LocalRedirect(Routes.ServiceProvidersIndex);
    }
}