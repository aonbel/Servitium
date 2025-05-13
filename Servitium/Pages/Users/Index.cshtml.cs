using Application.Features.Users.Queries;
using Domain.Entities.People;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Servitium.Pages.Users;

[Authorize(Roles = "Admin")]
public class Index(ISender sender) : PageModel
{
    public ICollection<IdentityUser> Data { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var query = new GetAllUsersQuery();
        var result = await sender.Send(query);
        
        if (result.IsError) return Page();

        Data = result.Value;

        return Page();
    }
}