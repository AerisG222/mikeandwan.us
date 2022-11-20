using StackExchange.Redis;
using Maw.Domain.Models;
using Maw.Domain.Models.Videos;

namespace Maw.Cache.Videos;

class CategorySerializer
    : BaseSerializer<Category>
{
    const string KEY_ID = "id";
    const string KEY_NAME = "name";
    const string KEY_YEAR = "year";
    const string KEY_CREATE_DATE = "create-date";
    const string KEY_IS_MISSING_GPS_DATA = "is-missing-gps-data";
    const string KEY_LATITUDE = "latitude";
    const string KEY_LONGITUDE = "longitude";
    const string KEY_VIDEO_COUNT = "video-count";
    const string KEY_TEASER_IMAGE_HEIGHT = "teaser-image-height";
    const string KEY_TEASER_IMAGE_WIDTH = "teaser-image-width";
    const string KEY_TEASER_IMAGE_PATH = "teaser-image-path";
    const string KEY_TEASER_IMAGE_SIZE = "teaser-image-size";
    const string KEY_TEASER_SQ_IMAGE_HEIGHT = "teaser-image-sq-height";
    const string KEY_TEASER_SQ_IMAGE_WIDTH = "teaser-image-sq-width";
    const string KEY_TEASER_SQ_IMAGE_PATH = "teaser-image-sq-path";
    const string KEY_TEASER_SQ_IMAGE_SIZE = "teaser-image-sq-size";
    const string KEY_TOTAL_DURATION = "total-duration";
    const string KEY_TOTAL_SIZE = "total-size";
    const string KEY_TOTAL_SIZE_THUMBNAIL = "total-size-thumbnail";
    const string KEY_TOTAL_SIZE_THUMBNAIL_SQ = "total-size-thumbnail-sq";
    const string KEY_TOTAL_SIZE_SCALED = "total-size-scaled";
    const string KEY_TOTAL_SIZE_FULL = "total-size-full";
    const string KEY_TOTAL_SIZE_RAW = "total-size-raw";

    static readonly RedisValue[] _hashFields = new RedisValue[]
    {
        KEY_ID,
        KEY_NAME,
        KEY_YEAR,
        KEY_CREATE_DATE,
        KEY_IS_MISSING_GPS_DATA,
        KEY_LATITUDE,
        KEY_LONGITUDE,
        KEY_VIDEO_COUNT,
        KEY_TEASER_IMAGE_HEIGHT,
        KEY_TEASER_IMAGE_WIDTH,
        KEY_TEASER_IMAGE_PATH,
        KEY_TEASER_IMAGE_SIZE,
        KEY_TEASER_SQ_IMAGE_HEIGHT,
        KEY_TEASER_SQ_IMAGE_WIDTH,
        KEY_TEASER_SQ_IMAGE_PATH,
        KEY_TEASER_SQ_IMAGE_SIZE,
        KEY_TOTAL_DURATION,
        KEY_TOTAL_SIZE,
        KEY_TOTAL_SIZE_THUMBNAIL,
        KEY_TOTAL_SIZE_THUMBNAIL_SQ,
        KEY_TOTAL_SIZE_SCALED,
        KEY_TOTAL_SIZE_FULL,
        KEY_TOTAL_SIZE_RAW
    };

    static readonly RedisValue[] _sortLookup = new RedisValue[]
    {
        "#",
        GetSortExternalLookup(VideoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_NAME),
        GetSortExternalLookup(VideoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_YEAR),
        GetSortExternalLookup(VideoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_CREATE_DATE),
        GetSortExternalLookup(VideoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_IS_MISSING_GPS_DATA),
        GetSortExternalLookup(VideoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_LATITUDE),
        GetSortExternalLookup(VideoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_LONGITUDE),
        GetSortExternalLookup(VideoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_VIDEO_COUNT),
        GetSortExternalLookup(VideoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TEASER_IMAGE_HEIGHT),
        GetSortExternalLookup(VideoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TEASER_IMAGE_WIDTH),
        GetSortExternalLookup(VideoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TEASER_IMAGE_PATH),
        GetSortExternalLookup(VideoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TEASER_IMAGE_SIZE),
        GetSortExternalLookup(VideoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TEASER_SQ_IMAGE_HEIGHT),
        GetSortExternalLookup(VideoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TEASER_SQ_IMAGE_WIDTH),
        GetSortExternalLookup(VideoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TEASER_SQ_IMAGE_PATH),
        GetSortExternalLookup(VideoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TEASER_SQ_IMAGE_SIZE),
        GetSortExternalLookup(VideoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TOTAL_DURATION),
        GetSortExternalLookup(VideoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TOTAL_SIZE),
        GetSortExternalLookup(VideoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TOTAL_SIZE_THUMBNAIL),
        GetSortExternalLookup(VideoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TOTAL_SIZE_THUMBNAIL_SQ),
        GetSortExternalLookup(VideoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TOTAL_SIZE_SCALED),
        GetSortExternalLookup(VideoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TOTAL_SIZE_FULL),
        GetSortExternalLookup(VideoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TOTAL_SIZE_RAW)
    };

    static readonly RedisValue[] _yearLookup = new RedisValue[]
    {
        GetSortExternalLookup(VideoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_YEAR)
    };

    public override RedisValue[] HashFields { get => _hashFields; }

    public override RedisValue[] SortLookupFields { get => _sortLookup; }

    public static RedisValue[] YearLookupField { get => _yearLookup; }

    public override HashEntry[] BuildHashSet(Category item)
    {
        List<HashEntry> entries = new()
        {
            new HashEntry(KEY_ID, item.Id),
            new HashEntry(KEY_NAME, item.Name),
            new HashEntry(KEY_YEAR, item.Year)
        };

        if (item.CreateDate != null)
        {
            entries.Add(new HashEntry(KEY_CREATE_DATE, SerializeDate((DateTime)item.CreateDate)));
        }

        entries.Add(new HashEntry(KEY_IS_MISSING_GPS_DATA, item.IsMissingGpsData));

        if(item.Latitude != null)
        {
            entries.Add(new HashEntry(KEY_LATITUDE, item.Latitude));
        }

        if(item.Longitude != null)
        {
            entries.Add(new HashEntry(KEY_LONGITUDE, item.Longitude));
        }

        entries.Add(new HashEntry(KEY_VIDEO_COUNT, item.VideoCount));

        if(item.TeaserImage != null)
        {
            entries.Add(new HashEntry(KEY_TEASER_IMAGE_HEIGHT, item.TeaserImage.Height));
            entries.Add(new HashEntry(KEY_TEASER_IMAGE_WIDTH, item.TeaserImage.Width));
            entries.Add(new HashEntry(KEY_TEASER_IMAGE_PATH, item.TeaserImage.Path));
            entries.Add(new HashEntry(KEY_TEASER_IMAGE_SIZE, item.TeaserImage.Size));
        }

        if(item.TeaserImageSq != null)
        {
            entries.Add(new HashEntry(KEY_TEASER_SQ_IMAGE_HEIGHT, item.TeaserImageSq.Height));
            entries.Add(new HashEntry(KEY_TEASER_SQ_IMAGE_WIDTH, item.TeaserImageSq.Width));
            entries.Add(new HashEntry(KEY_TEASER_SQ_IMAGE_PATH, item.TeaserImageSq.Path));
            entries.Add(new HashEntry(KEY_TEASER_SQ_IMAGE_SIZE, item.TeaserImageSq.Size));
        }

        entries.Add(new HashEntry(KEY_TOTAL_DURATION, item.TotalDuration));
        entries.Add(new HashEntry(KEY_TOTAL_SIZE, item.TotalSize));
        entries.Add(new HashEntry(KEY_TOTAL_SIZE_THUMBNAIL, item.TotalSizeThumbnail));
        entries.Add(new HashEntry(KEY_TOTAL_SIZE_THUMBNAIL_SQ, item.TotalSizeThumbnailSq));
        entries.Add(new HashEntry(KEY_TOTAL_SIZE_SCALED, item.TotalSizeScaled));
        entries.Add(new HashEntry(KEY_TOTAL_SIZE_FULL, item.TotalSizeFull));
        entries.Add(new HashEntry(KEY_TOTAL_SIZE_RAW, item.TotalSizeRaw));

        return entries.ToArray();
    }

    protected override Category ParseSingleInternal(ReadOnlySpan<RedisValue> values)
    {
        var createDate = values[3] == RedisValue.Null ? (DateTime?)null : DeserializeDate(values[3]!);

        return new Category
        {
            Id = (short)values[0],
            Name = values[1]!,
            Year = (short)values[2],
            CreateDate = createDate,
            IsMissingGpsData = (bool)values[4],
            Latitude = (float?)values[5],
            Longitude = (float?)values[6],
            VideoCount = (int?)values[7],
            TeaserImage = new MultimediaInfo
            {
                Height = (short)values[8],
                Width = (short)values[9],
                Path = values[10]!,
                Size = (long)values[11]
            },
            TeaserImageSq = new MultimediaInfo
            {
                Height = (short)values[12],
                Width = (short)values[13],
                Path = values[14]!,
                Size = (long)values[15]
            },
            TotalDuration = (int?)values[16],
            TotalSize = (long?)values[17],
            TotalSizeThumbnail = (long)values[18],
            TotalSizeThumbnailSq = (long)values[19],
            TotalSizeScaled = (long)values[20],
            TotalSizeFull = (long)values[21],
            TotalSizeRaw = (long)values[22]
        };
    }
}
