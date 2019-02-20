using System;


namespace MawApi.ViewModels.Videos
{
    public class VideoViewModel
    {
        public int Id { get; set; }
        public short CategoryId { get; set; }
        public DateTime CreateDate { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public int Duration { get; set; }
        public MultimediaAsset ThumbnailSq { get; set; }
        public MultimediaAsset Thumbnail { get; set; }
        public MultimediaAsset VideoScaled { get; set; }
		public MultimediaAsset VideoFull { get; set; }
        public MultimediaAsset VideoRaw { get; set; }
        public string Self { get; set; }
        public string CategoryLink { get; set; }
        public string CommentsLink { get; set; }
        public string RatingLink { get; set; }
    }
}
