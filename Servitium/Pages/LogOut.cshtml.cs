using System.ComponentModel.DataAnnotations;
using Application.Features.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Servitium.Infrastructure;
using Servitium.Infrastructure.PagesConstants;

namespace Servitium.Pages;

public class LogOutModel(TokenHandler tokenHandler) : PageModel
{
    public IActionResult OnGet() => Page();

    public async Task<IActionResult> OnPost(string? returnUrl = null)
    {
        tokenHandler.ClearTokens();
        
        return LocalRedirect(returnUrl ?? Url.Content("~/"));
    }
}