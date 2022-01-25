namespace MawApi.ViewModels.Videos;

public class VideoViewModel
{
    public int Id { get; set; }
    public short CategoryId { get; set; }
    public DateTime CreateDate { get; set; }
    public float? Latitude { get; set; }
    public float? Longitude { get; set; }
    public int Duration { get; set; }
    public MultimediaAsset ThumbnailSq { get; set; } = null!;
    public MultimediaAsset Thumbnail { get; set; } = null!;
    public MultimediaAsset VideoScaled { get; set; } = null!;
    public MultimediaAsset VideoFull { get; set; } = null!;
    public MultimediaAsset VideoRaw { get; set; } = null!;
    public string Self { get; set; } = null!;
    public string CategoryLink { get; set; } = null!;
    public string CommentsLink { get; set; } = null!;
    public string RatingLink { get; set; } = null!;
}
