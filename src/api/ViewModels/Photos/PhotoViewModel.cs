using System;


namespace MawApi.ViewModels.Photos
{
    public class PhotoViewModel
    {
        public int Id { get; set; }
        public short CategoryId { get; set; }
        public DateTime TakenDateUtc { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public Image ImageXsSq { get; set; }
        public Image ImageXs { get; set; }
        public Image ImageSm { get; set; }
		public Image ImageMd { get; set; }
        public Image ImageLg { get; set; }
        public Image ImagePrt { get; set; }
        public Image ImageRaw { get; set; }
        public string Self { get; set; }
        public string CategoryLink { get; set; }
    }
}
