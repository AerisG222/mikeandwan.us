using MawMvcApp.ViewModels.Navigation;


namespace MawMvcApp.ViewComponents
{
    public class PrimaryNavViewModel
    {
        public NavigationZone ActiveNavigationZone { get; set; }
        public bool AuthorizedForAdmin { get; set; }
    }
}
