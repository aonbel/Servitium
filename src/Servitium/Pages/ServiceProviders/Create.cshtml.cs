using System.ComponentModel.DataAnnotations;
using Application.Features.ServiceProviders.Commands;
using Domain.Entities.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Servitium.Infrastructure.PagesConstants;

namespace Servitium.Pages.ServiceProviders;

public class Create(ISender sender) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        [StringLength(Lengths.MaxServiceProviderName, ErrorMessage = ErrorMessages.ServiceProviderName,
            MinimumLength = Lengths.MinServiceProviderName)]
        [Display(Name = "Service provider name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(Lengths.MaxServiceProviderShortName, ErrorMessage = ErrorMessages.ServiceProviderShortName,
            MinimumLength = Lengths.MinServiceProviderShortName)]
        [Display(Name = "Service provider short name")]
        public string ShortName { get; set; } = string.Empty;

        [Required]
        [StringLength(Lengths.MaxServiceProviderAddress, ErrorMessage = ErrorMessages.ServiceProviderAddress,
            MinimumLength = Lengths.MinServiceProviderAddress)]
        [Display(Name = "Service provider address")]
        public string Address { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Work start time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public TimeOnly WorkBeginTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Work end time")]
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
            new TimeOnlySegment(Input.WorkBeginTime, Input.WorkEndTime),
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