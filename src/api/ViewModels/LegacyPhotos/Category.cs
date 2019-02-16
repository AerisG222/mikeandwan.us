namespace MawApi.ViewModels.LegacyPhotos
{
    public class Category
    {
        public short Id { get; set; }
        public string Name { get; set; }
		public short Year { get; set; }
        public bool HasGpsData { get; set; }
        public int PhotoCount { get; set; }
        public PhotoInfo TeaserPhotoInfo { get; set; }
    }
}
