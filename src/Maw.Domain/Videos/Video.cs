using System;


namespace Maw.Domain.Videos
{
	public class Video
	{
        public int Id { get; set; }
        public short CategoryId { get; set; }
        public DateTime CreateDate { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public short Duration { get; set; }
        public MultimediaInfo ThumbnailSq { get; set; }
        public MultimediaInfo Thumbnail { get; set; }
        public MultimediaInfo VideoScaled { get; set; }
		public MultimediaInfo VideoFull { get; set; }
        public MultimediaInfo VideoRaw { get; set; }
	}
}

