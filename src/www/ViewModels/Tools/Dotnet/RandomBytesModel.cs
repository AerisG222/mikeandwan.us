using System;
using System.ComponentModel.DataAnnotations;
using Maw.Domain.Utilities;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MawMvcApp.ViewModels.Tools.Dotnet;

public class RandomBytesModel
{
    [Required(ErrorMessage = "Please enter the number of bytes")]
    [Range(1, 8192)]
    [Display(Name = "Number of Bytes")]
    public int Size { get; set; }

    [Display(Name = "Random Bytes (hex)")]
    public string RandomBytes { get; private set; }

    [Display(Name = "Random Bytes (base64)")]
    public string RandomBytesBase64 { get; private set; }

    [BindNever]
    public bool HasErrors { get; set; }

    public void GenerateRandomness()
    {
        var randomBytes = CryptoUtils.GenerateRandom(Size);

        RandomBytes = StringUtils.ToHexString(randomBytes).Substring(0, Size);
        RandomBytesBase64 = Convert.ToBase64String(randomBytes);
    }
}
