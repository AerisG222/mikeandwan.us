namespace Maw.Domain.Models.Photos;

public class Photo
{
    public int Id { get; set; }
    public short CategoryId { get; set; }
    public DateTime? CreateDate { get; set; }
    public float? Latitude { get; set; }
    public float? Longitude { get; set; }
    public MultimediaInfo XsInfo { get; set; } = null!;
    public MultimediaInfo XsSqInfo { get; set; } = null!;
    public MultimediaInfo SmInfo { get; set; } = null!;
    public MultimediaInfo MdInfo { get; set; } = null!;
    public MultimediaInfo LgInfo { get; set; } = null!;
    public MultimediaInfo PrtInfo { get; set; } = null!;
    public MultimediaInfo SrcInfo { get; set; } = null!;
}
