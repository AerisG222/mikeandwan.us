using Maw.Domain.Photos;


namespace api.Models.Photos
{
    public class Photo
    {
        public int Id { get; set; }
        public short CategoryId { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public PhotoInfo XsInfo { get; set; }
        public PhotoInfo SmInfo { get; set; }
		public PhotoInfo MdInfo { get; set; }
        public PhotoInfo LgInfo { get; set; }
        public PhotoInfo PrtInfo { get; set; }
        public string Self { get; set; }
        public string CategoryLink { get; set; }
    }
}
