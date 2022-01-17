using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MawMvcApp.ViewModels.Tools.FileSize;

public class FileSizeViewModel
{
    public double Size { get; set; }
    public string SizeScale { get; set; }

    [BindNever]
    public List<Result> Results { get; private set; }

    [BindNever]
    public string ErrorMessage { get; set; }

    public void Calculate()
    {
        var results = new List<Result>();
        var unit = FileSizeUnit.AllUnits
            .SingleOrDefault(x => string.Equals(x.Name, SizeScale, StringComparison.OrdinalIgnoreCase));

        if (unit == null)
        {
            ErrorMessage = "Please specify a valid file size unit";
            return;
        }

        double sizeInBytes = Size * unit.BytesInUnit;

        foreach (var u in FileSizeUnit.AllUnits)
        {
            results.Add(new Result(u.Name, sizeInBytes / u.BytesInUnit));
        }

        Results = results;
    }
}
