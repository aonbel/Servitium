using System.ComponentModel.DataAnnotations;
using Application.Features.Clients.Commands;
using Application.Features.Persons.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Servitium.Infrastructure.PagesConstants;

namespace Servitium.Pages.Clients;

public class Create(ISender sender) : PageModel
{
    public SelectList GenderList { get; set; } = new(Genders.GendersList);

    [BindProperty] public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date of birth")]
        public DateOnly Birthday { get; set; }

        [Required]
        public string Gender { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnPostAsync(string id, string? returnUrl = null)
    {
        returnUrl ??= Url.Content(Routes.Index);

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var getPersonByUserIdQuery = new GetPersonByUserIdQuery(id);

        var getPersonByUserIdQueryResponse = await sender.Send(getPersonByUserIdQuery);

        if (getPersonByUserIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getPersonByUserIdQueryResponse.Error.Code,
                getPersonByUserIdQueryResponse.Error.Message);
            return Page();
        }
        
        var person = getPersonByUserIdQueryResponse.Value;

        var create = new CreateClientCommand(person.Id ?? 0, Input.Birthday, Input.Gender);

        var response = await sender.Send(create);

        if (response.IsError)
        {
            ModelState.AddModelError(response.Error.Code, response.Error.Message);
            return Page();
        }

        return LocalRedirect(returnUrl);
    }
}