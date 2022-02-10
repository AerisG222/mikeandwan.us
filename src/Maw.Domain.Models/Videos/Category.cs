namespace Maw.Domain.Models.Videos;

public record Category
{
    public short Id { get; set; }
    public string Name { get; set; } = null!;
    public short Year { get; set; }
    public DateTime? CreateDate { get; set; }
    public float? Latitude { get; set; }
    public float? Longitude { get; set; }
    public int? VideoCount { get; set; }
    public int? TotalDuration { get; set; }
    public long? TotalSizeThumbnail { get; set; }
    public long? TotalSizeThumbnailSq { get; set; }
    public long? TotalSizeScaled { get; set; }
    public long? TotalSizeFull { get; set; }
    public long? TotalSizeRaw { get; set; }
    public long? TotalSize { get; set; }
    public MultimediaInfo TeaserImage { get; set; } = null!;
    public MultimediaInfo TeaserImageSq { get; set; } = null!;
    public bool IsMissingGpsData { get; set; }
}
