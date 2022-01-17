using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MawMvcApp.ViewModels.Tools.Bandwidth;

public class BandwidthViewModel
{
    [Display(Name = "File Size")]
    public double Size { get; set; }
    public char SizeScale { get; set; }
    public char TimeScale { get; set; }

    [BindNever]
    public List<BandwidthSizeResult> Results { get; private set; }

    [BindNever]
    public string ErrorMessage { get; set; }

    public void Calculate()
    {
        double timeInSeconds;
        double sizeInBytes;
        var results = new List<BandwidthSizeResult>();

        // determine the divisor based on the time interval
        switch (TimeScale)
        {
            case 'm':
                timeInSeconds = 60;
                break;
            case 'h':
                timeInSeconds = 60 * 60;
                break;
            case 's':
                timeInSeconds = 1;
                break;
            case 'd':
                timeInSeconds = 60 * 60 * 24;
                break;
            default:
                ErrorMessage = "Invalid time interval specified";
                return;
        }

        // now determine the full size of the file, in bits, based
        // on the file size scale
        switch (SizeScale)
        {
            case 'b':
                sizeInBytes = Size * 8;
                break;
            case 'k':
                sizeInBytes = Size * 1024 * 8;
                break;
            case 'm':
                sizeInBytes = Size * 1024 * 1024 * 8;
                break;
            case 'g':
                sizeInBytes = Size * 1024 * 1024 * 1024 * 8;
                break;
            default:
                ErrorMessage = "Invalid file size scale specified";
                return;
        }

        foreach (var size in BandwidthSizeInfo.AllSizes)
        {
            results.Add(new BandwidthSizeResult(size.Name, size.Speed, (sizeInBytes / size.Bps) / timeInSeconds));
        }

        Results = results;
    }
}
