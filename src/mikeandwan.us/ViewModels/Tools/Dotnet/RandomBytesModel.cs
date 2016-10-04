using System;
using System.ComponentModel.DataAnnotations;
using Maw.Domain.Utilities;


namespace MawMvcApp.ViewModels.Tools.Dotnet
{
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
		
		
		public void GenerateRandomness()
		{
			var crypto = new Crypto();

			var randomBytes = crypto.GenerateRandom(Size);

			RandomBytes = StringUtils.ToHexString(randomBytes).Substring(0, Size);
			RandomBytesBase64 = Convert.ToBase64String(randomBytes);
		}
	}
}

