using MawMvcApp.ViewModels.Navigation;

namespace MawMvcApp.ViewComponents;

public class AccountStatusViewModel
{
    public NavigationZone ActiveNavigationZone { get; set; }
    public bool IsAuthenticated { get; set; }
}
