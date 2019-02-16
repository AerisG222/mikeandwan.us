using System;


namespace MawApi.ViewModels.Photos
{
    public class PhotoViewModel
    {
        public int Id { get; set; }
        public short CategoryId { get; set; }
        public DateTime CreateDate { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public MultimediaAsset ImageXsSq { get; set; }
        public MultimediaAsset ImageXs { get; set; }
        public MultimediaAsset ImageSm { get; set; }
		public MultimediaAsset ImageMd { get; set; }
        public MultimediaAsset ImageLg { get; set; }
        public MultimediaAsset ImagePrt { get; set; }
        public MultimediaAsset ImageSrc { get; set; }
        public string Self { get; set; }
        public string CategoryLink { get; set; }
    }
}
