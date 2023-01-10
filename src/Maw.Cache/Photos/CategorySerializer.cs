using StackExchange.Redis;
using Maw.Domain.Models;
using Maw.Domain.Models.Photos;

namespace Maw.Cache.Photos;

sealed class CategorySerializer
    : BaseSerializer<Category>
{
    const string KEY_ID = "id";
    const string KEY_NAME = "name";
    const string KEY_YEAR = "year";
    const string KEY_CREATE_DATE = "create-date";
    const string KEY_IS_MISSING_GPS_DATA = "is-missing-gps-data";
    const string KEY_LATITUDE = "latitude";
    const string KEY_LONGITUDE = "longitude";
    const string KEY_PHOTO_COUNT = "photo-count";
    const string KEY_TEASER_IMAGE_HEIGHT = "teaser-image-height";
    const string KEY_TEASER_IMAGE_WIDTH = "teaser-image-width";
    const string KEY_TEASER_IMAGE_PATH = "teaser-image-path";
    const string KEY_TEASER_IMAGE_SIZE = "teaser-image-size";
    const string KEY_TEASER_SQ_IMAGE_HEIGHT = "teaser-image-sq-height";
    const string KEY_TEASER_SQ_IMAGE_WIDTH = "teaser-image-sq-width";
    const string KEY_TEASER_SQ_IMAGE_PATH = "teaser-image-sq-path";
    const string KEY_TEASER_SQ_IMAGE_SIZE = "teaser-image-sq-size";
    const string KEY_TOTAL_SIZE = "total-size";
    const string KEY_TOTAL_SIZE_SRC = "total-size-src";
    const string KEY_TOTAL_SIZE_PRT = "total-size-prt";
    const string KEY_TOTAL_SIZE_LG = "total-size-lg";
    const string KEY_TOTAL_SIZE_MD = "total-size-md";
    const string KEY_TOTAL_SIZE_SM = "total-size-sm";
    const string KEY_TOTAL_SIZE_XS = "total-size-xs";
    const string KEY_TOTAL_SIZE_XS_SQ = "total-size-xs-sq";

    static readonly RedisValue[] _hashFields = new RedisValue[]
    {
        KEY_ID,
        KEY_NAME,
        KEY_YEAR,
        KEY_CREATE_DATE,
        KEY_IS_MISSING_GPS_DATA,
        KEY_LATITUDE,
        KEY_LONGITUDE,
        KEY_PHOTO_COUNT,
        KEY_TEASER_IMAGE_HEIGHT,
        KEY_TEASER_IMAGE_WIDTH,
        KEY_TEASER_IMAGE_PATH,
        KEY_TEASER_IMAGE_SIZE,
        KEY_TEASER_SQ_IMAGE_HEIGHT,
        KEY_TEASER_SQ_IMAGE_WIDTH,
        KEY_TEASER_SQ_IMAGE_PATH,
        KEY_TEASER_SQ_IMAGE_SIZE,
        KEY_TOTAL_SIZE,
        KEY_TOTAL_SIZE_SRC,
        KEY_TOTAL_SIZE_PRT,
        KEY_TOTAL_SIZE_LG,
        KEY_TOTAL_SIZE_MD,
        KEY_TOTAL_SIZE_SM,
        KEY_TOTAL_SIZE_XS,
        KEY_TOTAL_SIZE_XS_SQ
    };

    static readonly RedisValue[] _sortLookup = new RedisValue[]
    {
        "#",
        GetSortExternalLookup(PhotoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_NAME),
        GetSortExternalLookup(PhotoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_YEAR),
        GetSortExternalLookup(PhotoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_CREATE_DATE),
        GetSortExternalLookup(PhotoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_IS_MISSING_GPS_DATA),
        GetSortExternalLookup(PhotoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_LATITUDE),
        GetSortExternalLookup(PhotoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_LONGITUDE),
        GetSortExternalLookup(PhotoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_PHOTO_COUNT),
        GetSortExternalLookup(PhotoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TEASER_IMAGE_HEIGHT),
        GetSortExternalLookup(PhotoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TEASER_IMAGE_WIDTH),
        GetSortExternalLookup(PhotoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TEASER_IMAGE_PATH),
        GetSortExternalLookup(PhotoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TEASER_IMAGE_SIZE),
        GetSortExternalLookup(PhotoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TEASER_SQ_IMAGE_HEIGHT),
        GetSortExternalLookup(PhotoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TEASER_SQ_IMAGE_WIDTH),
        GetSortExternalLookup(PhotoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TEASER_SQ_IMAGE_PATH),
        GetSortExternalLookup(PhotoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TEASER_SQ_IMAGE_SIZE),
        GetSortExternalLookup(PhotoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TOTAL_SIZE),
        GetSortExternalLookup(PhotoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TOTAL_SIZE_SRC),
        GetSortExternalLookup(PhotoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TOTAL_SIZE_PRT),
        GetSortExternalLookup(PhotoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TOTAL_SIZE_LG),
        GetSortExternalLookup(PhotoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TOTAL_SIZE_MD),
        GetSortExternalLookup(PhotoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TOTAL_SIZE_SM),
        GetSortExternalLookup(PhotoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TOTAL_SIZE_XS),
        GetSortExternalLookup(PhotoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_TOTAL_SIZE_XS_SQ)
    };

    static readonly RedisValue[] _yearLookup = new RedisValue[]
    {
        GetSortExternalLookup(PhotoKeys.CATEGORY_HASH_KEY_PATTERN, KEY_YEAR)
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

        entries.Add(new HashEntry(KEY_PHOTO_COUNT, item.PhotoCount));

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

        entries.Add(new HashEntry(KEY_TOTAL_SIZE, item.TotalSize));
        entries.Add(new HashEntry(KEY_TOTAL_SIZE_SRC, item.TotalSizeSrc));
        entries.Add(new HashEntry(KEY_TOTAL_SIZE_PRT, item.TotalSizePrt));
        entries.Add(new HashEntry(KEY_TOTAL_SIZE_LG, item.TotalSizeLg));
        entries.Add(new HashEntry(KEY_TOTAL_SIZE_MD, item.TotalSizeMd));
        entries.Add(new HashEntry(KEY_TOTAL_SIZE_SM, item.TotalSizeSm));
        entries.Add(new HashEntry(KEY_TOTAL_SIZE_XS, item.TotalSizeXs));
        entries.Add(new HashEntry(KEY_TOTAL_SIZE_XS_SQ, item.TotalSizeXsSq));

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
            PhotoCount = (int)values[7],
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
            TotalSize = (long)values[16],
            TotalSizeSrc = (long)values[17],
            TotalSizePrt = (long)values[18],
            TotalSizeLg = (long)values[19],
            TotalSizeMd = (long)values[20],
            TotalSizeSm = (long)values[21],
            TotalSizeXs = (long)values[22],
            TotalSizeXsSq = (long)values[23]
        };
    }
}
