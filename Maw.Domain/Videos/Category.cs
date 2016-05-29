namespace Maw.Domain.Videos
{
	public class Category
	{
		public short Id { get; set; }
		public string Name { get; set; }
		public short Year { get; set; }
        public VideoInfo TeaserThumbnail { get; set; }
	}
}

