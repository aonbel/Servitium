using Application.Features.Persons.Queries;
using Application.Features.Users.Queries;
using Domain.Entities.People;
using Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Servitium.Extensions;

namespace Servitium.Pages.Persons.Manager;

[Authorize(Roles = ApplicationRoles.Manager)]
public class Index(ISender sender) : PageModel
{
    public class DataModel
    {
        public List<string> RolesForEachPerson { get; set; } = [];
        
        public List<string> FullNamesForEachPerson { get; set; } = [];
        
        public List<string> UserIdsForEachPerson { get; set; } = [];
        
        public List<bool> CanGiveSpecialistRole { get; set; } = [];
    }
    
    public DataModel Data { get; set; } = new();
    
    public async Task<IActionResult> OnGetAsync()
    {
        var query = new GetAllPersonsQuery();
        
        var response = await sender.Send(query);

        if (response.IsError)
        {
            ModelState.AddModelError(response.Error.Code, response.Error.Message);
            return LocalRedirect(Routes.Index);
        }

        var persons = response.Value;

        foreach (var person in persons)
        {
            var getUserByIdQuery = new GetRolesByUserIdQuery(person.UserId);
            
            var getUserByIdQueryResponse = await sender.Send(getUserByIdQuery);

            if (getUserByIdQueryResponse.IsError)
            {
                ModelState.AddModelError(getUserByIdQueryResponse.Error);
                return LocalRedirect(Routes.Index);
            }
            
            var roles = getUserByIdQueryResponse.Value;
            
            Data.UserIdsForEachPerson.Add(person.UserId);
            Data.CanGiveSpecialistRole.Add(!roles.Contains(ApplicationRoles.Specialist));
            Data.FullNamesForEachPerson.Add($"{person.FirstName} {person.LastName}");
            Data.RolesForEachPerson.Add(string.Join(", ", roles));
        }
        
        return Page();
    }
}