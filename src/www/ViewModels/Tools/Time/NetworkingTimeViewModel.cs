using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace MawMvcApp.ViewModels.Tools.Time
{
    public class NetworkingTimeViewModel
    {
        [Display(Name = "Length of Time")]
        public double LengthOfTime { get; set; }
        [Display(Name = "Time Unit")]
        public string TimeUnit { get; set; }

        [BindNever]
        public List<Result> Results { get; private set; }

        [BindNever]
        public string ErrorMessage { get; set; }



        public void Calculate()
        {
            double timeInSeconds = 0;
            var results = new List<Result>();

            var timeScale = TimeScale.AllScales
                .SingleOrDefault(x => string.Equals(x.Name, TimeUnit, StringComparison.OrdinalIgnoreCase));

            if(timeScale == null)
            {
                ErrorMessage = "Please select a valid time unit";
                return;
            }

            timeInSeconds = LengthOfTime * timeScale.SecondsInUnit;

            foreach(var scale in TimeScale.AllScales)
            {
                results.Add(new Result(scale.Name, timeInSeconds / scale.SecondsInUnit));
            }

            Results = results;
        }
    }
}
