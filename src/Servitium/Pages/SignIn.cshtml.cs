using System.ComponentModel.DataAnnotations;
using Application.Features.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Servitium.Infrastructure;
using Servitium.Infrastructure.PagesConstants;

namespace Servitium.Pages;

public class SignInModel(ISender sender, TokenHandler tokenHandler) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string ReturnUrl { get; set; } = string.Empty;

    public class InputModel
    {
        [Required]
        [StringLength(Constraints.MaxUsername, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = Constraints.MinUsername)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(Constraints.MaxPassword, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = Constraints.MinPassword)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid) return Page();

        var command = new SignInCommand(
            Input.Username,
            Input.Password);

        var result = await sender.Send(command);
        if (result.IsSuccess)
        {
            var responce = result.Value;
            tokenHandler.SetTokensIntoCookie(responce.AccessToken, responce.RefreshToken);

            return LocalRedirect(returnUrl);
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt");
        return Page();
    }
}