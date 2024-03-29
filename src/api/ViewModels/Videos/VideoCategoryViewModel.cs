namespace MawApi.ViewModels.Videos;

public class VideoCategoryViewModel
{
    public short Id { get; set; }
    public string Name { get; set; } = null!;
    public short Year { get; set; }
    public DateTime? CreateDate { get; set; }
    public float? Latitude { get; set; }
    public float? Longitude { get; set; }
    public int? VideoCount { get; set; }
    public long? TotalDuration { get; set; }
    public long? TotalSizeThumbnail { get; set; }
    public long? TotalSizeThumbnailSq { get; set; }
    public long? TotalSizeScaled { get; set; }
    public long? TotalSizeFull { get; set; }
    public long? TotalSizeRaw { get; set; }
    public long? TotalSize { get; set; }
    public MultimediaAsset TeaserImage { get; set; } = null!;
    public MultimediaAsset TeaserImageSq { get; set; } = null!;
    public string Self { get; set; } = null!;
    public string VideosLink { get; set; } = null!;
    public bool IsMissingGpsData { get; set; }
}
