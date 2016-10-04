namespace MawMvcApp.ViewModels.Navigation
{
    public class SidebarModel
    {
        public string Group { get; set; }
        public string Item { get; set; }


        public SidebarModel()
        {
            Group = string.Empty;
            Item = string.Empty;
        }
    }
}
