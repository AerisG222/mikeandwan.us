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
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public int PhotoCount { get; set; }
        public long TotalImageSizeLg { get; set; }
        public long TotalImageSizeMd { get; set; }
        public long TotalImageSizeSm { get; set; }
        public long TotalImageSizeXs { get; set; }
        public long TotalImageSizeXsSq { get; set; }
        public MultimediaAsset TeaserImage { get; set; }
        public MultimediaAsset TeaserImageSq { get; set; }
        public string Self { get; set; }
    }
}
