using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace MawMvcApp.ViewModels.Tools
{
    public class ColorConverterModel
        : IValidatableObject
    {
        [Display(Name = "Color Code (hex)")]
        public string HexColorCode { get; set; }

        [Display(Name = "Red (0-255)")]
        public byte? RedComponent { get; set; }

        [Display(Name = "Green (0-255)")]
        public byte? GreenComponent { get; set; }

        [Display(Name = "Blue (0-255)")]
        public byte? BlueComponent { get; set; }

        public ColorConversionMode ConversionMode { get; set; }

        [BindNever]
        public bool HasErrors { get; set; }

        [BindNever]
        public string ErrorMessage { get; set; }

        [BindNever]
        public string HtmlColorCode { get; set; }


        public IEnumerable<ValidationResult> Validate (ValidationContext validationContext)
        {
            switch(ConversionMode)
            {
                case ColorConversionMode.FromHex:
                    if(string.IsNullOrEmpty(HexColorCode))
                    {
                        yield return new ValidationResult("Color code must be provided.", new string[] {nameof(HexColorCode)});
                    }
                    else if(!ValidateHexCode())
                    {
                        yield return new ValidationResult("Color code is not a valid hex format.  Please enter this as #RRGGBB", new string [] { nameof(HexColorCode) });
                    }

                    break;
                case ColorConversionMode.FromComponents:
                    if(RedComponent == null)
                    {
                        yield return new ValidationResult("Red component must be provided.", new string[] {nameof(RedComponent)});
                    }
                    if(GreenComponent == null)
                    {
                        yield return new ValidationResult("Green component must be provided.", new string[] {nameof(GreenComponent)});
                    }
                    if(BlueComponent == null)
                    {
                        yield return new ValidationResult("Blue component must be provided.", new string[] {nameof(BlueComponent)});
                    }
                    break;
                default:
                    throw new InvalidOperationException("A valid encoding mode was not specified.");
            }
        }


        public void Convert()
        {
            switch(ConversionMode)
            {
                case ColorConversionMode.FromHex:
                    try
                    {
                        var code = NormalizeHexCode();

                        switch(code.Length) {
                            case 3:
                                RedComponent = ToByte(string.Concat(code[0], code[0]));
                                GreenComponent = ToByte(string.Concat(code[1], code[1]));
                                BlueComponent = ToByte(string.Concat(code[2], code[2]));
                                break;
                            case 6:
                                RedComponent = ToByte(string.Concat(code[0], code[1]));
                                GreenComponent = ToByte(string.Concat(code[2], code[3]));
                                BlueComponent = ToByte(string.Concat(code[4], code[5]));
                                break;
                            default:
                                ErrorMessage = "Invalid color value";
                                break;
                        }

                        HtmlColorCode = GetHtmlColorCodeFromComponents();
                    }
                    catch {
                        ErrorMessage = "Sorry, there was an error converting the color you provided, please make sure it is a valid format";
                    }

                    break;
                case ColorConversionMode.FromComponents:
                    HexColorCode = GetHtmlColorCodeFromComponents();
                    HtmlColorCode = HexColorCode;

                    break;
                default:
                    throw new Exception("Make sure conversion mode is properly specified");
            }
        }


        bool ValidateHexCode()
        {
            var code = NormalizeHexCode();
            var max = int.Parse("FFFFFF", NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            int val;

            if(int.TryParse(code, NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out val)) {
                if(val < 0 || val > max) {
                    return false;
                }

                return true;
            }

            return false;
        }


        string GetHtmlColorCodeFromComponents() {
            return string.Concat("#", ToHex(RedComponent), ToHex(GreenComponent), ToHex(BlueComponent));
        }


        string NormalizeHexCode()
        {
            if(HexColorCode.StartsWith("#", true, CultureInfo.InvariantCulture)) {
                return HexColorCode.Substring(1);
            }

            return HexColorCode;
        }


        byte ToByte(string hex)
        {
            return (byte)int.Parse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }


        string ToHex(byte? val)
        {
            return ((byte)val).ToString("X2", CultureInfo.InvariantCulture).ToLowerInvariant();
        }
    }
}

