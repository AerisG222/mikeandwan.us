using System;

namespace MawApi.ViewModels.Photos;

public class PhotoViewModel
{
    public int Id { get; set; }
    public short CategoryId { get; set; }
    public DateTime CreateDate { get; set; }
    public float? Latitude { get; set; }
    public float? Longitude { get; set; }
    public PhotoMultimediaAsset ImageXsSq { get; set; } = null!;
    public PhotoMultimediaAsset ImageXs { get; set; } = null!;
    public PhotoMultimediaAsset ImageSm { get; set; } = null!;
    public PhotoMultimediaAsset ImageMd { get; set; } = null!;
    public PhotoMultimediaAsset ImageLg { get; set; } = null!;
    public PhotoMultimediaAsset ImagePrt { get; set; } = null!;
    public PhotoMultimediaAsset ImageSrc { get; set; } = null!;
    public string Self { get; set; } = null!;
    public string CategoryLink { get; set; } = null!;
    public string CommentsLink { get; set; } = null!;
    public string ExifLink { get; set; } = null!;
    public string RatingLink { get; set; } = null!;
}
