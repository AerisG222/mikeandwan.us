using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MawMvcApp.ViewModels.Tools.Dotnet;

public class HtmlEncodeDecodeModel
    : IValidatableObject
{
    [DataType(DataType.MultilineText)]
    [Display(Name = "Encoded String")]
    public string EncodedString { get; set; }

    [DataType(DataType.MultilineText)]
    [Display(Name = "Decoded String")]
    public string DecodedString { get; set; }

    public EncodeMode Mode { get; set; }

    [BindNever]
    public bool HasErrors { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errorList = new List<ValidationResult>();

        switch (Mode)
        {
            case EncodeMode.Decode:
                if (string.IsNullOrEmpty(EncodedString))
                {
                    errorList.Add(new ValidationResult("Encoded string must be provided.", new string[] { nameof(EncodedString) }));
                }
                break;
            case EncodeMode.Encode:
                if (string.IsNullOrEmpty(DecodedString))
                {
                    errorList.Add(new ValidationResult("Decoded string must be provided.", new string[] { nameof(DecodedString) }));
                }
                break;
            default:
                throw new InvalidOperationException("A valid encoding mode was not specified.");
        }

        return errorList;
    }

    public void Encode()
    {
        EncodedString = WebUtility.HtmlEncode(DecodedString);
    }

    public void Decode()
    {
        DecodedString = WebUtility.HtmlDecode(EncodedString);
    }
}
