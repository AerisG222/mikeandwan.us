using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MawMvcApp.ViewModels.Navigation;


namespace MawMvcApp.ViewComponents
{
    public class AccountStatus
        : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(NavigationZone activeZone)
        {
            var model = new AccountStatusViewModel {
                ActiveNavigationZone = activeZone,
                IsAuthenticated = HttpContext.User.Identity.IsAuthenticated
            };

            return Task.FromResult<IViewComponentResult>(View(model));
        }
    }
}
