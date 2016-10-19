namespace Maw.Domain.Photos.ThreeD
{
    public class Category3D
    {
        public short Id { get; set; }
        public string Name { get; set; }
		public short Year { get; set; }
        public Image3D TeaserImage { get; set; }
    }
}
