using System;

namespace Maw.Domain.Blogs;

public class Blog
{
    public short Id { get; set; }
    public string Title { get; set; }
    public string Copyright { get; set; }
    public string Description { get; set; }
    public DateTime LastPostDate { get; set; }
}
