using StackExchange.Redis;
using Maw.Domain.Models;
using Maw.Domain.Models.Videos;

namespace Maw.Cache.Videos;

class VideoSerializer
    : BaseSerializer<Video>
{
    const string KEY_ID = "id";
    const string KEY_CATEGORY_ID = "category-id";
    const string KEY_CREATE_DATE = "create-date";
    const string KEY_LATITUDE = "latitude";
    const string KEY_LONGITUDE = "longitude";
    const string KEY_DURATION = "duration";
    const string KEY_THUMBNAIL_SQ_HEIGHT = "thumbnail-sq-height";
    const string KEY_THUMBNAIL_SQ_WIDTH = "thumbnail-sq-width";
    const string KEY_THUMBNAIL_SQ_PATH = "thumbnail-sq-path";
    const string KEY_THUMBNAIL_SQ_SIZE = "thumbnail-sq-size";
    const string KEY_THUMBNAIL_HEIGHT = "thumbnail-height";
    const string KEY_THUMBNAIL_WIDTH = "thumbnail-width";
    const string KEY_THUMBNAIL_PATH = "thumbnail-path";
    const string KEY_THUMBNAIL_SIZE = "thumbnail-size";
    const string KEY_SCALED_HEIGHT = "scaled-height";
    const string KEY_SCALED_WIDTH = "scaled-width";
    const string KEY_SCALED_PATH = "scaled-path";
    const string KEY_SCALED_SIZE = "scaled-size";
    const string KEY_FULL_HEIGHT = "full-height";
    const string KEY_FULL_WIDTH = "full-width";
    const string KEY_FULL_PATH = "full-path";
    const string KEY_FULL_SIZE = "full-size";
    const string KEY_RAW_HEIGHT = "raw-height";
    const string KEY_RAW_WIDTH = "raw-width";
    const string KEY_RAW_PATH = "raw-path";
    const string KEY_RAW_SIZE = "raw-size";

    static readonly RedisValue[] _hashFields = new RedisValue[]
    {
        KEY_ID,
        KEY_CATEGORY_ID,
        KEY_CREATE_DATE,
        KEY_LATITUDE,
        KEY_LONGITUDE,
        KEY_DURATION,
        KEY_THUMBNAIL_HEIGHT,
        KEY_THUMBNAIL_WIDTH,
        KEY_THUMBNAIL_PATH,
        KEY_THUMBNAIL_SIZE,
        KEY_THUMBNAIL_SQ_HEIGHT,
        KEY_THUMBNAIL_SQ_WIDTH,
        KEY_THUMBNAIL_SQ_PATH,
        KEY_THUMBNAIL_SQ_SIZE,
        KEY_SCALED_HEIGHT,
        KEY_SCALED_WIDTH,
        KEY_SCALED_PATH,
        KEY_SCALED_SIZE,
        KEY_FULL_HEIGHT,
        KEY_FULL_WIDTH,
        KEY_FULL_PATH,
        KEY_FULL_SIZE,
        KEY_RAW_HEIGHT,
        KEY_RAW_WIDTH,
        KEY_RAW_PATH,
        KEY_RAW_SIZE
    };

    static readonly RedisValue[] _sortLookup = new RedisValue[]
    {
        "#",
        GetSortExternalLookup(VideoKeys.VIDEO_HASH_KEY_PATTERN, KEY_CATEGORY_ID),
        GetSortExternalLookup(VideoKeys.VIDEO_HASH_KEY_PATTERN, KEY_CREATE_DATE),
        GetSortExternalLookup(VideoKeys.VIDEO_HASH_KEY_PATTERN, KEY_LATITUDE),
        GetSortExternalLookup(VideoKeys.VIDEO_HASH_KEY_PATTERN, KEY_LONGITUDE),
        GetSortExternalLookup(VideoKeys.VIDEO_HASH_KEY_PATTERN, KEY_DURATION),
        GetSortExternalLookup(VideoKeys.VIDEO_HASH_KEY_PATTERN, KEY_THUMBNAIL_HEIGHT),
        GetSortExternalLookup(VideoKeys.VIDEO_HASH_KEY_PATTERN, KEY_THUMBNAIL_WIDTH),
        GetSortExternalLookup(VideoKeys.VIDEO_HASH_KEY_PATTERN, KEY_THUMBNAIL_PATH),
        GetSortExternalLookup(VideoKeys.VIDEO_HASH_KEY_PATTERN, KEY_THUMBNAIL_SIZE),
        GetSortExternalLookup(VideoKeys.VIDEO_HASH_KEY_PATTERN, KEY_THUMBNAIL_SQ_HEIGHT),
        GetSortExternalLookup(VideoKeys.VIDEO_HASH_KEY_PATTERN, KEY_THUMBNAIL_SQ_WIDTH),
        GetSortExternalLookup(VideoKeys.VIDEO_HASH_KEY_PATTERN, KEY_THUMBNAIL_SQ_PATH),
        GetSortExternalLookup(VideoKeys.VIDEO_HASH_KEY_PATTERN, KEY_THUMBNAIL_SQ_SIZE),
        GetSortExternalLookup(VideoKeys.VIDEO_HASH_KEY_PATTERN, KEY_SCALED_HEIGHT),
        GetSortExternalLookup(VideoKeys.VIDEO_HASH_KEY_PATTERN, KEY_SCALED_WIDTH),
        GetSortExternalLookup(VideoKeys.VIDEO_HASH_KEY_PATTERN, KEY_SCALED_PATH),
        GetSortExternalLookup(VideoKeys.VIDEO_HASH_KEY_PATTERN, KEY_SCALED_SIZE),
        GetSortExternalLookup(VideoKeys.VIDEO_HASH_KEY_PATTERN, KEY_FULL_HEIGHT),
        GetSortExternalLookup(VideoKeys.VIDEO_HASH_KEY_PATTERN, KEY_FULL_WIDTH),
        GetSortExternalLookup(VideoKeys.VIDEO_HASH_KEY_PATTERN, KEY_FULL_PATH),
        GetSortExternalLookup(VideoKeys.VIDEO_HASH_KEY_PATTERN, KEY_FULL_SIZE),
        GetSortExternalLookup(VideoKeys.VIDEO_HASH_KEY_PATTERN, KEY_RAW_HEIGHT),
        GetSortExternalLookup(VideoKeys.VIDEO_HASH_KEY_PATTERN, KEY_RAW_WIDTH),
        GetSortExternalLookup(VideoKeys.VIDEO_HASH_KEY_PATTERN, KEY_RAW_PATH),
        GetSortExternalLookup(VideoKeys.VIDEO_HASH_KEY_PATTERN, KEY_RAW_SIZE)
    };

    public override RedisValue[] HashFields { get => _hashFields; }

    public override RedisValue[] SortLookupFields { get => _sortLookup; }

    public override HashEntry[] BuildHashSet(Video item)
    {
        List<HashEntry> entries = new();

        entries.Add(new HashEntry(KEY_ID, item.Id));
        entries.Add(new HashEntry(KEY_CATEGORY_ID, item.CategoryId));

        if(item.CreateDate != null)
        {
            entries.Add(new HashEntry(KEY_CREATE_DATE, SerializeDate((DateTime)item.CreateDate)));
        }

        if(item.Latitude != null)
        {
            entries.Add(new HashEntry(KEY_LATITUDE, item.Latitude));
        }

        if(item.Longitude != null)
        {
            entries.Add(new HashEntry(KEY_LONGITUDE, item.Longitude));
        }

        entries.Add(new HashEntry(KEY_DURATION, item.Duration));

        if(item.Thumbnail != null)
        {
            entries.Add(new HashEntry(KEY_THUMBNAIL_HEIGHT, item.Thumbnail.Height));
            entries.Add(new HashEntry(KEY_THUMBNAIL_WIDTH, item.Thumbnail.Width));
            entries.Add(new HashEntry(KEY_THUMBNAIL_PATH, item.Thumbnail.Path));
            entries.Add(new HashEntry(KEY_THUMBNAIL_SIZE, item.Thumbnail.Size));
        }

        if(item.ThumbnailSq != null)
        {
            entries.Add(new HashEntry(KEY_THUMBNAIL_SQ_HEIGHT, item.ThumbnailSq.Height));
            entries.Add(new HashEntry(KEY_THUMBNAIL_SQ_WIDTH, item.ThumbnailSq.Width));
            entries.Add(new HashEntry(KEY_THUMBNAIL_SQ_PATH, item.ThumbnailSq.Path));
            entries.Add(new HashEntry(KEY_THUMBNAIL_SQ_SIZE, item.ThumbnailSq.Size));
        }

        if(item.VideoScaled != null)
        {
            entries.Add(new HashEntry(KEY_SCALED_HEIGHT, item.VideoScaled.Height));
            entries.Add(new HashEntry(KEY_SCALED_WIDTH, item.VideoScaled.Width));
            entries.Add(new HashEntry(KEY_SCALED_PATH, item.VideoScaled.Path));
            entries.Add(new HashEntry(KEY_SCALED_SIZE, item.VideoScaled.Size));
        }

        if(item.VideoFull != null)
        {
            entries.Add(new HashEntry(KEY_FULL_HEIGHT, item.VideoFull.Height));
            entries.Add(new HashEntry(KEY_FULL_WIDTH, item.VideoFull.Width));
            entries.Add(new HashEntry(KEY_FULL_PATH, item.VideoFull.Path));
            entries.Add(new HashEntry(KEY_FULL_SIZE, item.VideoFull.Size));
        }

        if(item.VideoRaw != null)
        {
            entries.Add(new HashEntry(KEY_RAW_HEIGHT, item.VideoRaw.Height));
            entries.Add(new HashEntry(KEY_RAW_WIDTH, item.VideoRaw.Width));
            entries.Add(new HashEntry(KEY_RAW_PATH, item.VideoRaw.Path));
            entries.Add(new HashEntry(KEY_RAW_SIZE, item.VideoRaw.Size));
        }

        return entries.ToArray();
    }

    protected override Video ParseSingleInternal(ReadOnlySpan<RedisValue> values)
    {
        var createDate = values[2] == RedisValue.Null ? (DateTime?)null : DeserializeDate(values[2]);

        return new Video
        {
            Id = (short)values[0],
            CategoryId = (short)values[1],
            CreateDate = createDate,
            Latitude = (float?)values[3],
            Longitude = (float?)values[4],
            Duration = (short)values[5],
            Thumbnail = new MultimediaInfo
            {
                Height = (short)values[6],
                Width = (short)values[7],
                Path = values[8],
                Size = (long)values[9]
            },
            ThumbnailSq = new MultimediaInfo
            {
                Height = (short)values[10],
                Width = (short)values[11],
                Path = values[12],
                Size = (long)values[13]
            },
            VideoScaled = new MultimediaInfo
            {
                Height = (short)values[14],
                Width = (short)values[15],
                Path = values[16],
                Size = (long)values[17]
            },
            VideoFull = new MultimediaInfo
            {
                Height = (short)values[18],
                Width = (short)values[19],
                Path = values[20],
                Size = (long)values[21]
            },
            VideoRaw = new MultimediaInfo
            {
                Height = (short)values[22],
                Width = (short)values[23],
                Path = values[24],
                Size = (long)values[25]
            }
        };
    }
}
