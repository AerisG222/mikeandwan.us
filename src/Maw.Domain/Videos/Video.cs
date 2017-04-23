namespace Maw.Domain.Videos
{
	public class Video
	{
        public short Id { get; set; }
        public short Duration { get; set; }
		public Category Category { get; set; }
        public VideoInfo ScaledVideo { get; set; }
        public VideoInfo FullsizeVideo { get; set; }
        public VideoInfo ThumbnailVideo { get; set; }
	}
}

