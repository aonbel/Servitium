using Application.Features.ServiceProviders.Queries;
using Domain.Entities.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Servitium.Pages.ServiceProviders;

public class Details(ISender sender) : PageModel
{
    public List<SelectListItem> DaysOfWeekList { get; set; } = Enum
        .GetValues<DayOfWeek>()
        .Select(d => new SelectListItem
        {
            Text = d.ToString(),
            Value = ((int)d).ToString()
        })
        .ToList();
    
    public ServiceProviderDetails Data { get; set; } = new ();
    
    public class ServiceProviderDetails
    {
        public string ShortName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string WorkTime { get; set; } = string.Empty;
        public ICollection<DayOfWeek> WorkDays { get; set; } = []; 
        public string Contacts { get; set; } = string.Empty;
    }
    
    public async Task<IActionResult> OnGetAsync(int id)
    {
        var query = new GetServiceProviderByIdQuery(id);
        var response = await sender.Send(query);

        if (response.IsError)
        {
            ModelState.AddModelError(response.Error.Code, response.Error.Message);
            return LocalRedirect(Routes.Index);
        }
        
        var serviceProvider = response.Value;

        Data = new ServiceProviderDetails
        {
            ShortName = serviceProvider.ShortName,
            FullName = serviceProvider.Name,
            Address = serviceProvider.Address,
            WorkTime = serviceProvider.WorkTime.ToString(),
            WorkDays = serviceProvider.WorkDays,
            Contacts = string.Join(',', serviceProvider.Contacts.ToArray()),
        };
        
        return Page();
    }
}