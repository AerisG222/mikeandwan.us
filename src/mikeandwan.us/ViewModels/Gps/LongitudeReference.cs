using System.ComponentModel.DataAnnotations;


namespace MawMvcApp.ViewModels.Gps
{
    public enum LongitudeReference
    {
        [Display(Name = "East")]
        East,

        [Display(Name = "West (-)")]
        West
    }
}
