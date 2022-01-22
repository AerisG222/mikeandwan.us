namespace MawApi.ViewModels;

public class MultimediaAsset
{
    public short Height { get; set; }
    public short Width { get; set; }
    public string Url { get; set; } = null!;
    public long Size { get; set; }
}
