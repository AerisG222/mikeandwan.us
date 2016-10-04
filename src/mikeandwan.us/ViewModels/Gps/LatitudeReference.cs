using System.ComponentModel.DataAnnotations;


namespace MawMvcApp.ViewModels.Gps
{
    public enum LatitudeReference
    {
        [Display(Name = "North")]
        North,

        [Display(Name = "South (-)")]
        South
    }
}
