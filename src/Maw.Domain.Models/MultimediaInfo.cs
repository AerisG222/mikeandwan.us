namespace Maw.Domain.Models;

public record MultimediaInfo
{
    public short Height { get; set; }
    public short Width { get; set; }
    public string Path { get; set; } = null!;
    public long Size { get; set; }
}
