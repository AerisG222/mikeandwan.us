namespace Maw.Domain.Photos.ThreeD
{
    public class Photo3D
    {
		public int Id { get; set; }
        public Image3D XsImage { get; set; }
        public Image3D MdImage { get; set; }
        public Image3D LgImage { get; set; }
    }
}
