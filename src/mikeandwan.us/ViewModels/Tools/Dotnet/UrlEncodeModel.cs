using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MawMvcApp.ViewModels.Tools.Dotnet
{
	public class UrlEncodeModel
		: IValidatableObject
	{
		[Display(Name = "Encoded String")]
		[DataType(DataType.MultilineText)]
		public string EncodedString { get; set; }
		
		[Display(Name = "Decoded String")]
		[DataType(DataType.MultilineText)]
		public string DecodedString { get; set; }
		
		
		public EncodeMode Mode { get; set; }
		
		[BindNever]
		public bool HasErrors { get; set; }
		
		public UrlEncodeModel ()
		{
			// do nothing
		}
		
		
		public IEnumerable<ValidationResult> Validate (ValidationContext validationContext)
		{
			var errorList = new List<ValidationResult>();
			
			switch(Mode)
			{
				case EncodeMode.Decode:
					if(string.IsNullOrEmpty(EncodedString))
					{
						errorList.Add(new ValidationResult("Please specify the encoded string.", new string[] { nameof(EncodedString) }));
					}
					break;
				case EncodeMode.Encode:
					if(string.IsNullOrEmpty(DecodedString))
					{
						errorList.Add(new ValidationResult("Please specify the decoded string.", new string[] { nameof(DecodedString) }));
					}
					break;
				default:
					throw new InvalidOperationException("A proper EncodeMode must be specified!");
			}
			
			return errorList;
		}
		
		
		public void PerformCoding()
	    {
			if(Mode == EncodeMode.Decode)
			{
                DecodedString = WebUtility.UrlDecode(EncodedString);
			}
	        else if(Mode == EncodeMode.Encode)
			{
                EncodedString = WebUtility.UrlEncode(DecodedString);
			}
	    }
	}
}

