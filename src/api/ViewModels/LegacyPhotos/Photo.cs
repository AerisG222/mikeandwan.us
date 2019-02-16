namespace MawApi.ViewModels.LegacyPhotos
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
    }
}
