namespace MawMvcApp.ViewModels
{
    public class FrameworkIdentifier
	{
        public bool IsFullFramework
        {
            get
            {
#if NET452
                return true;
#else
                return false;
#endif
            }
        }
    }
}
