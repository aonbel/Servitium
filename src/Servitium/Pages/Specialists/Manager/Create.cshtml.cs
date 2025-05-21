using System.ComponentModel.DataAnnotations;
using Application.Features.Persons.Queries;
using Application.Features.ServiceProviderManagers.Queries;
using Application.Features.Services.Queries;
using Application.Features.Specialists.Commands;
using Domain.Entities.Core;
using Infrastructure.Authentification;
using Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Servitium.Extensions;

namespace Servitium.Pages.Specialists.Manager;

[Authorize(Roles = ApplicationRoles.Manager)]
public class Create(ISender sender) : PageModel
{
    public SelectList ServicesSelectList { get; set; }
    
    [BindProperty] public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        [Display(Name = "Price per hour")]
        [DataType(DataType.Currency)]
        public decimal PricePerHour { get; set; }

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

        [Required]
        [Display(Name = "Work days")]
        public List<DayOfWeek> WorkDays { get; set; } = [];

        [Required]
        [Display(Name = "Contacts")]
        public string Contacts { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Location in service provider")]
        public string Location { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Services")]
        public List<int> ServiceIds { get; set; } = new();
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var getAllServicesQuery = new GetAllServicesQuery();
        
        var getAllServicesQueryResponse = await sender.Send(getAllServicesQuery);

        if (getAllServicesQueryResponse.IsError)
        {
            ModelState.AddModelError(getAllServicesQueryResponse.Error);
            return Page();
        }

        var services = getAllServicesQueryResponse.Value;
        
        ServicesSelectList = new SelectList(services, "Id", "ShortName");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id, string? returnUrl = null)
    {
        returnUrl ??= Url.Content(Routes.Index);

        var getSpecialistPersonByUserIdQuery = new GetPersonByUserIdQuery(id);

        var getSpecialistPersonByUserIdQueryResponse = await sender.Send(getSpecialistPersonByUserIdQuery);

        if (getSpecialistPersonByUserIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getSpecialistPersonByUserIdQueryResponse.Error.Code,
                getSpecialistPersonByUserIdQueryResponse.Error.Message);

            return Page();
        }

        var getManagerPersonByUserIdQuery = new GetPersonByUserIdQuery(User.GetUserId());

        var getManagerPersonByUserIdQueryResponse = await sender.Send(getManagerPersonByUserIdQuery);

        if (getManagerPersonByUserIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getManagerPersonByUserIdQueryResponse.Error.Code,
                getManagerPersonByUserIdQueryResponse.Error.Message);

            return Page();
        }

        var managerPerson = getManagerPersonByUserIdQueryResponse.Value;

        var getServiceProviderManagerByPersonIdQuery =
            new GetServiceProviderManagerByPersonIdQuery(managerPerson.Id ?? 0);

        var getServiceProviderManagerByPersonIdQueryResponse =
            await sender.Send(getServiceProviderManagerByPersonIdQuery);

        if (getServiceProviderManagerByPersonIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getServiceProviderManagerByPersonIdQueryResponse.Error.Code,
                getServiceProviderManagerByPersonIdQueryResponse.Error.Message);

            return Page();
        }

        var serviceProviderManager = getServiceProviderManagerByPersonIdQueryResponse.Value;

        var specialistPerson = getSpecialistPersonByUserIdQueryResponse.Value;

        var createSpecialistCommand = new CreateSpecialistCommand(
            specialistPerson.Id ?? 0,
            serviceProviderManager.ServiceProviderId,
            Input.PricePerHour,
            new TimeOnlySegment(Input.WorkBeginTime, Input.WorkEndTime),
            Input.WorkDays,
            Input.Contacts.Split(','),
            Input.Location,
            Input.ServiceIds);

        var createSpecialistCommandResponse = await sender.Send(createSpecialistCommand);

        if (createSpecialistCommandResponse.IsError)
        {
            ModelState.AddModelError(createSpecialistCommandResponse.Error.Code,
                createSpecialistCommandResponse.Error.Message);

            return Page();
        }

        return LocalRedirect(returnUrl);
    }
}