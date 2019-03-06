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
        public PhotoMultimediaAsset ImageXsSq { get; set; }
        public PhotoMultimediaAsset ImageXs { get; set; }
        public PhotoMultimediaAsset ImageSm { get; set; }
		public PhotoMultimediaAsset ImageMd { get; set; }
        public PhotoMultimediaAsset ImageLg { get; set; }
        public PhotoMultimediaAsset ImagePrt { get; set; }
        public PhotoMultimediaAsset ImageSrc { get; set; }
        public string Self { get; set; }
        public string CategoryLink { get; set; }
        public string CommentsLink { get; set; }
        public string ExifLink { get; set;}
        public string RatingLink { get; set; }
    }
}
