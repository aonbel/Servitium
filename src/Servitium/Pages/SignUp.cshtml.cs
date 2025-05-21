using System.ComponentModel.DataAnnotations;
using Application.Features.Clients.Commands;
using Application.Features.Persons.Commands;
using Application.Features.Users.Commands;
using Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Servitium.Infrastructure;
using Servitium.Infrastructure.PagesConstants;

namespace Servitium.Pages;

public class SignUpModel(ISender sender, TokenHandler tokenHandler) : PageModel
{
    [BindProperty] public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public class InputModel
    {
        [Required]
        [StringLength(Lengths.MaxUsername, ErrorMessage = ErrorMessages.UserName, MinimumLength = Lengths.MinUsername)]
        [Display(Name = "User name")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(Lengths.MaxFirstName, ErrorMessage = ErrorMessages.UserFirstName,
            MinimumLength = Lengths.MinFirstName)]
        [Display(Name = "First name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(Lengths.MaxMiddleName, ErrorMessage = ErrorMessages.UserMiddleName,
            MinimumLength = Lengths.MinMiddleName)]
        [Display(Name = "Middle name")]
        public string MiddleName { get; set; } = string.Empty;

        [Required]
        [StringLength(Lengths.MaxLastName, ErrorMessage = ErrorMessages.UserLastName,
            MinimumLength = Lengths.MinLastName)]
        [Display(Name = "Last name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Phone number")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [StringLength(Lengths.MaxPassword, ErrorMessage = ErrorMessages.Password, MinimumLength = Lengths.MinPassword)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = ErrorMessages.ConfirmPassword)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content(Routes.Index);
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content(Routes.Index);

        if (!ModelState.IsValid) return Page();

        var signUpCommand = new SignUpCommand(
            Input.Username,
            Input.Password,
            [ApplicationRoles.Unauthenticated]);

        var signUpCommandResult = await sender.Send(signUpCommand);

        if (signUpCommandResult.IsError)
        {
            ModelState.AddModelError(string.Empty, signUpCommandResult.Error.Message);
            return Page();
        }

        var userSignUpResponce = signUpCommandResult.Value;

        tokenHandler.SetTokensIntoCookie(userSignUpResponce.AccessToken, userSignUpResponce.RefreshToken);

        var createPersonCommand = new CreatePersonCommand(
            userSignUpResponce.User.Id,
            Input.FirstName,
            Input.LastName,
            Input.MiddleName,
            Input.Phone,
            Input.Email);

        var createPersonCommandResponce = await sender.Send(createPersonCommand);

        if (createPersonCommandResponce.IsError)
        {
            // TODO add user deletion
            ModelState.AddModelError(createPersonCommandResponce.Error.Code, createPersonCommandResponce.Error.Message);
            return Page();
        }

        return LocalRedirect(returnUrl);
    }
}