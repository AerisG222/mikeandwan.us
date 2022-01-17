using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MawMvcApp.ViewModels.Tools;

public class ByteCounterViewModel
{
    [Display(Name = "Text to Count")]
    public string Text { get; set; }

    [BindNever]
    public int Bytes { get; set; }
    [BindNever]
    public double Kilobytes { get; set; }
    [BindNever]
    public double Megabytes { get; set; }

    public void Calculate()
    {
        if (!string.IsNullOrEmpty(Text))
        {
            Bytes = Text.Length;
            Kilobytes = Text.Length / 1024;
            Megabytes = Text.Length / (1024 * 1024);
        }
    }
}
