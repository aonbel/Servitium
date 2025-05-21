using Domain.Abstractions.Result;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Servitium.Extensions;

public static class ModelStateExtensions
{
    public static void AddModelError(this ModelStateDictionary modelState, Error error) =>
        modelState.AddModelError(error.Code, error.Message);
}