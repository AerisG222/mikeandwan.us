namespace Maw.Domain.Photos
{
    public class CategoryPhotoCount
    {
		public short Year { get; set; }
        public short CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int PhotoCount { get; set; }
    }
}
