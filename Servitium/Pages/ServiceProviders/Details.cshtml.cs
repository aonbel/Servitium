using Application.Features.ServiceProviders.Queries;
using Domain.Entities.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Servitium.Pages.ServiceProviders;

public class Details(ISender sender) : PageModel
{
    public ServiceProviderDetails Data { get; set; } = new ();
    
    public class ServiceProviderDetails
    {
        public string ShortName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public Coordinates? Coordinates { get; set; }
        public TimeOnlySegment? WorkTime { get; set; }
        public ICollection<DayOfWeek> WorkDays { get; set; } = []; 
        public ICollection<string> Contacts { get; set; } = [];
    }
    
    public async Task<IActionResult> OnGetAsync(int serviceProviderId)
    {
        var query = new GetServiceProviderByIdQuery(serviceProviderId);
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
            Coordinates = serviceProvider.Coordinates,
            WorkTime = serviceProvider.WorkTime,
            WorkDays = serviceProvider.WorkDays,
            Contacts = serviceProvider.Contacts,
        };
        
        return Page();
    }
}