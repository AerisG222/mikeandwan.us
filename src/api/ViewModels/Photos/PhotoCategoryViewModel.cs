using System;
using Maw.Domain.Photos;


namespace MawApi.ViewModels.Photos
{
    public class PhotoCategoryViewModel
    {
        public short Id { get; set; }
        public string Name { get; set; }
		public short Year { get; set; }
        public DateTime TakenDateUtc { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int PhotoCount { get; set; }
        public Image TeaserImage { get; set; }
        public string Self { get; set; }
    }
}
