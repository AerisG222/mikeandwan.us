using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace MawMvcApp.ViewModels.Tools.Dotnet
{
	public class DateDiff
	{
		[Required(ErrorMessage = "Please enter the start date")]
		[Display(Name = "Start Date")]
		[DataType(DataType.Date)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
		public DateTime StartDate { get; set; }
		
		[Required(ErrorMessage = "Please enter the end date")]
		[Display(Name = "End Date")]
		[DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
		public DateTime EndDate { get; set; }
		
		[BindNever]
		public bool ShowResults { get; set; }
		
		[BindNever]
		public TimeSpan Diff {	get { return EndDate - StartDate; } }
		
		[BindNever]
		public double TotalMilliseconds { get { return Diff.TotalMilliseconds; } }
		
		[BindNever]
		public double TotalSeconds { get { return Diff.TotalSeconds; } }
		
		[BindNever]
		public double TotalMinutes { get { return Diff.TotalMinutes; } }
		
		[BindNever]
		public double TotalHours { get { return Diff.TotalHours; } }
		
		[BindNever]
		public double TotalDays { get { return Diff.TotalDays; } }
		
		[BindNever]
		public double TotalWeeks { get { return Diff.TotalDays / 7.0; } }
		
		[BindNever]
		public double TotalMonths { get { return Diff.TotalDays / 30.0; } }
		
		[BindNever]
		public double TotalYears { get { return Diff.TotalDays / 365.25; } }
	}
}

