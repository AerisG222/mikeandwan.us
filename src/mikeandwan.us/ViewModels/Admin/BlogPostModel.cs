using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace MawMvcApp.ViewModels.Admin
{
	public class BlogPostModel
	{
		[Required(ErrorMessage = "Please enter the title")]
		public string Title { get; set; }
		
		[Required(ErrorMessage = "Please enter the description")]
		public string Description { get; set; }
		
		[Required(ErrorMessage = "Please enter the publish date")]
		[Display(Name = "Publish Date")]
		public DateTime PublishDate { get; set; }
		
		[BindNever]
		public bool Success { get; set; }

		[BindNever]
		public bool WasAttempted { get; set; }
		
		[BindNever]
		public bool Preview { get; set; }
		
		[Required]
		public BlogPostAction Behavior { get; set; }
	}
}