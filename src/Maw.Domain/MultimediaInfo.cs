namespace Maw.Domain;

public class MultimediaInfo
{
    public short Height { get; set; }
    public short Width { get; set; }
    public string Path { get; set; } = null!;
    public long Size { get; set; }
}
