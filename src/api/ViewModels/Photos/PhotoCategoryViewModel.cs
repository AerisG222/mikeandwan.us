using System;
using Maw.Domain.Photos;


namespace MawApi.ViewModels.Photos
{
    public class PhotoCategoryViewModel
    {
        public short Id { get; set; }
        public string Name { get; set; }
		public short Year { get; set; }
        public DateTime CreateDate { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public int PhotoCount { get; set; }
        public long TotalSizeLg { get; set; }
        public long TotalSizeMd { get; set; }
        public long TotalSizeSm { get; set; }
        public long TotalSizeXs { get; set; }
        public long TotalSizeXsSq { get; set; }
        public MultimediaAsset TeaserImage { get; set; }
        public MultimediaAsset TeaserImageSq { get; set; }
        public string Self { get; set; }
    }
}
