using Application.Features.Clients.Queries;
using Application.Features.Persons.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Servitium.Extensions;

namespace Servitium.Pages.Clients.Specialist;

public class Details(ISender sender) : PageModel
{
    public DataModel Data { get; set; } = new();
    
    public class DataModel
    {
        public string FirstName { get; set; } = string.Empty;
        
        public string LastName { get; set; } = string.Empty;
        
        public string MiddleName { get; set; } = string.Empty;
        
        public string PhoneNumber { get; set; } = string.Empty;
        
        public string Gender { get; set; } = string.Empty;
        
        public DateOnly BirthDate { get; set; }
    }
    
    public async Task<IActionResult> OnGetAsync(int id)
    {
        var getClientByIdQuery = new GetClientByIdQuery(id);
        
        var getClientByIdQueryResponse = await sender.Send(getClientByIdQuery);

        if (getClientByIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getClientByIdQueryResponse.Error);
            return RedirectToPage(Routes.Index);
        }
        
        var client = getClientByIdQueryResponse.Value;
        
        var getPersonByIdQuery = new GetPersonByIdQuery(client.PersonId);
        
        var getPersonByIdQueryResponse = await sender.Send(getPersonByIdQuery);

        if (getPersonByIdQueryResponse.IsError)
        {
            ModelState.AddModelError(getClientByIdQueryResponse.Error);
            return RedirectToPage(Routes.Index);
        }
        
        var clientPerson = getPersonByIdQueryResponse.Value;

        Data = new DataModel
        {
            FirstName = clientPerson.FirstName,
            LastName = clientPerson.LastName,
            MiddleName = clientPerson.MiddleName,
            PhoneNumber = clientPerson.Phone,
            Gender = client.Gender,
            BirthDate = client.Birthday
        };
        
        return Page();
    }
}