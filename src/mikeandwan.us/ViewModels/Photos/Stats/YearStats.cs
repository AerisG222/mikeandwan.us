using System.Collections.Generic;


namespace MawMvcApp.ViewModels.Photos.Stats
{
	public class YearStats
	{
        public short Year { get; set; }
        public IList<CategoryStats> CategoryStats { get; } = new List<CategoryStats>();
    }
}
