using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace MawMvcApp.ViewModels.Tools.Dotnet
{
	public class RegexViewModel
	{
		[Required(ErrorMessage = "Please enter the regex pattern")]
		[DataType(DataType.MultilineText)]
		[Display(Name = "Regular Expression Pattern")]
		public string Pattern { get; set; }

		[Required(ErrorMessage = "Please enter the input string")]
		[DataType(DataType.MultilineText)]
		[Display(Name = "Input String")]
		public string Input { get; set; }


		[Display(Name = "Culture Invariant")]
		public bool OptionCultureInvariant { get; set; }

		[Display(Name = "ECMA Script")]
		public bool OptionEcmaScript { get; set; }

		[Display(Name = "Explicit Capture")]
		public bool OptionExplicitCapture { get; set; }

		[Display(Name = "Ignore Case")]
		public bool OptionIgnoreCase { get; set; }

		[Display(Name = "Ignore Pattern Whitespace")]
		public bool OptionIgnorePatternWhitespace { get; set; }

		[Display(Name = "Multiline")]
		public bool OptionMultiline { get; set; }

		[Display(Name = "None")]
		public bool OptionNone { get; set; }

		[Display(Name = "Right to Left")]
		public bool OptionRightToLeft { get; set; }

		[Display(Name = "Single Line")]
		public bool OptionSingleLine { get; set; }

		[BindNever]
		public bool HasErrors { get; set; }

		[BindNever]
		public MatchCollection RegexMatches { get; private set; }

		[BindNever]
		public Regex Regex { get; set; }


		public void Execute()
		{
			Regex = PrepareRegex();
			RegexMatches = Regex.Matches(Input.Trim());
		}


		Regex PrepareRegex()
	    {
	        RegexOptions options = RegexOptions.None;

	        if(OptionCultureInvariant)
	        {
	            options = options | RegexOptions.CultureInvariant;
	        }
			//ECMA Script can only be combined with Ignore Case, Multiline, and Culture Invariant
	        if(OptionEcmaScript)
	        {
	            options = options | RegexOptions.ECMAScript;
	        }
	        if(OptionExplicitCapture)
	        {
	            options = options | RegexOptions.ExplicitCapture;
	        }
	        if(OptionIgnoreCase)
	        {
	            options = options | RegexOptions.IgnoreCase;
	        }
	        if(OptionIgnorePatternWhitespace)
	        {
	            options = options | RegexOptions.IgnorePatternWhitespace;
	        }
	        if(OptionMultiline)
	        {
	            options = options | RegexOptions.Multiline;
	        }
	        if(OptionRightToLeft)
	        {
	            options = options | RegexOptions.RightToLeft;
	        }
	        if(OptionSingleLine)
	        {
	            options = options | RegexOptions.Singleline;
	        }

	        return new Regex(Pattern, options);
	    }
	}
}
