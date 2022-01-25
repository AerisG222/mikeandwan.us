namespace Maw.Domain.Models.Blogs;

public class Blog
{
    public short Id { get; set; }
    public string Title { get; set; } = null!;
    public string Copyright { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime LastPostDate { get; set; }
}
