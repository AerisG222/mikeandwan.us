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
		
		
		public string RandomBytes { get; private set; }
		
		
		public void GenerateRandomness()
		{
			var crypto = new Crypto();
			
			RandomBytes = crypto.GenerateRandom(Size);
		}
	}
}

