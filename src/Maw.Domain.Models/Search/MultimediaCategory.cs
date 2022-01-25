using SolrNet.Attributes;

namespace Maw.Domain.Models.Search;

public class MultimediaCategory
{
    [SolrUniqueKey("solr_id")]
    public string SolrId { get; set; } = null!;

    [SolrField("id")]
    public int Id { get; set; }

    [SolrField("year")]
    public int Year { get; set; }

    [SolrField("name")]
    public string Name { get; set; } = null!;

    [SolrField("type")]
    public string MultimediaType { get; set; } = null!;

    [SolrField("teaser_photo_height")]
    public int TeaserPhotoHeight { get; set; }

    [SolrField("teaser_photo_width")]
    public int TeaserPhotoWidth { get; set; }

    [SolrField("teaser_photo_path")]
    public string TeaserPhotoPath { get; set; } = null!;

    [SolrField("teaser_photo_sq_height")]
    public int TeaserPhotoSqHeight { get; set; }

    [SolrField("teaser_photo_sq_width")]
    public int TeaserPhotoSqWidth { get; set; }

    [SolrField("teaser_photo_sq_path")]
    public string TeaserPhotoSqPath { get; set; } = null!;

    [SolrField("score")]
    public double Score { get; set; }
}
