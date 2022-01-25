namespace Maw.Domain.Models.Blogs;

public class Post
{
    public short Id { get; set; }
    public short BlogId { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime PublishDate { get; set; }
}
