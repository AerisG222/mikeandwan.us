namespace Maw.Domain.Models.Videos;

public record Video
{
    public int Id { get; set; }
    public short CategoryId { get; set; }
    public DateTime? CreateDate { get; set; }
    public float? Latitude { get; set; }
    public float? Longitude { get; set; }
    public short Duration { get; set; }
    public MultimediaInfo ThumbnailSq { get; set; } = null!;
    public MultimediaInfo Thumbnail { get; set; } = null!;
    public MultimediaInfo VideoScaled { get; set; } = null!;
    public MultimediaInfo VideoFull { get; set; } = null!;
    public MultimediaInfo VideoRaw { get; set; } = null!;
}
