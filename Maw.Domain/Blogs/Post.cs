using System;


namespace Maw.Domain.Blogs
{
	public class Post
	{
		public short Id { get; set; }
		public short BlogId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public DateTime PublishDate { get; set; }
	}
}

