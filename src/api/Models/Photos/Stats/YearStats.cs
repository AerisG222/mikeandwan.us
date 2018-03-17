using System.Collections.Generic;


namespace MawApi.Models.Photos.Stats
{
	public class YearStats
	{
        public short Year { get; set; }
        public IList<CategoryStats> CategoryStats { get; } = new List<CategoryStats>();
    }
}
