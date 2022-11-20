using StackExchange.Redis;
using Maw.Domain.Models;
using Maw.Domain.Models.Photos;

namespace Maw.Cache.Photos;

class PhotoSerializer
    : BaseSerializer<Photo>
{
    const string KEY_ID = "id";
    const string KEY_CATEGORY_ID = "category-id";
    const string KEY_CREATE_DATE = "create-date";
    const string KEY_LATITUDE = "latitude";
    const string KEY_LONGITUDE = "longitude";
    const string KEY_XS_HEIGHT = "xs-height";
    const string KEY_XS_WIDTH = "xs-width";
    const string KEY_XS_PATH = "xs-path";
    const string KEY_XS_SIZE = "xs-size";
    const string KEY_XS_SQ_HEIGHT = "xs-sq-height";
    const string KEY_XS_SQ_WIDTH = "xs-sq-width";
    const string KEY_XS_SQ_PATH = "xs-sq-path";
    const string KEY_XS_SQ_SIZE = "xs-sq-size";
    const string KEY_SM_HEIGHT = "sm-height";
    const string KEY_SM_WIDTH = "sm-width";
    const string KEY_SM_PATH = "sm-path";
    const string KEY_SM_SIZE = "sm-size";
    const string KEY_MD_HEIGHT = "md-height";
    const string KEY_MD_WIDTH = "md-width";
    const string KEY_MD_PATH = "md-path";
    const string KEY_MD_SIZE = "md-size";
    const string KEY_LG_HEIGHT = "lg-height";
    const string KEY_LG_WIDTH = "lg-width";
    const string KEY_LG_PATH = "lg-path";
    const string KEY_LG_SIZE = "lg-size";
    const string KEY_PRT_HEIGHT = "prt-height";
    const string KEY_PRT_WIDTH = "prt-width";
    const string KEY_PRT_PATH = "prt-path";
    const string KEY_PRT_SIZE = "prt-size";
    const string KEY_SRC_HEIGHT = "src-height";
    const string KEY_SRC_WIDTH = "src-width";
    const string KEY_SRC_PATH = "src-path";
    const string KEY_SRC_SIZE = "src-size";

    static readonly RedisValue[] _hashFields = new RedisValue[]
    {
        KEY_ID,
        KEY_CATEGORY_ID,
        KEY_CREATE_DATE,
        KEY_LATITUDE,
        KEY_LONGITUDE,
        KEY_XS_HEIGHT,
        KEY_XS_WIDTH,
        KEY_XS_PATH,
        KEY_XS_SIZE,
        KEY_XS_SQ_HEIGHT,
        KEY_XS_SQ_WIDTH,
        KEY_XS_SQ_PATH,
        KEY_XS_SQ_SIZE,
        KEY_SM_HEIGHT,
        KEY_SM_WIDTH,
        KEY_SM_PATH,
        KEY_SM_SIZE,
        KEY_MD_HEIGHT,
        KEY_MD_WIDTH,
        KEY_MD_PATH,
        KEY_MD_SIZE,
        KEY_LG_HEIGHT,
        KEY_LG_WIDTH,
        KEY_LG_PATH,
        KEY_LG_SIZE,
        KEY_PRT_HEIGHT,
        KEY_PRT_WIDTH,
        KEY_PRT_PATH,
        KEY_PRT_SIZE,
        KEY_SRC_HEIGHT,
        KEY_SRC_WIDTH,
        KEY_SRC_PATH,
        KEY_SRC_SIZE
    };

    static readonly RedisValue[] _sortLookup = new RedisValue[]
    {
        "#",
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_CATEGORY_ID),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_CREATE_DATE),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_LATITUDE),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_LONGITUDE),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_XS_HEIGHT),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_XS_WIDTH),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_XS_PATH),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_XS_SIZE),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_XS_SQ_HEIGHT),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_XS_SQ_WIDTH),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_XS_SQ_PATH),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_XS_SQ_SIZE),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_SM_HEIGHT),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_SM_WIDTH),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_SM_PATH),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_SM_SIZE),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_MD_HEIGHT),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_MD_WIDTH),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_MD_PATH),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_MD_SIZE),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_LG_HEIGHT),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_LG_WIDTH),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_LG_PATH),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_LG_SIZE),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_PRT_HEIGHT),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_PRT_WIDTH),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_PRT_PATH),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_PRT_SIZE),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_SRC_HEIGHT),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_SRC_WIDTH),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_SRC_PATH),
        GetSortExternalLookup(PhotoKeys.PHOTO_HASH_KEY_PATTERN, KEY_SRC_SIZE)
    };

    public override RedisValue[] HashFields { get => _hashFields; }
    public override RedisValue[] SortLookupFields { get => _sortLookup; }

    public override HashEntry[] BuildHashSet(Photo item)
    {
        List<HashEntry> entries = new()
        {
            new HashEntry(KEY_ID, item.Id),
            new HashEntry(KEY_CATEGORY_ID, item.CategoryId)
        };

        if (item.CreateDate != null)
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

        if(item.XsInfo != null)
        {
            entries.Add(new HashEntry(KEY_XS_HEIGHT, item.XsInfo.Height));
            entries.Add(new HashEntry(KEY_XS_WIDTH, item.XsInfo.Width));
            entries.Add(new HashEntry(KEY_XS_PATH, item.XsInfo.Path));
            entries.Add(new HashEntry(KEY_XS_SIZE, item.XsInfo.Size));
        }

        if(item.XsSqInfo != null)
        {
            entries.Add(new HashEntry(KEY_XS_SQ_HEIGHT, item.XsSqInfo.Height));
            entries.Add(new HashEntry(KEY_XS_SQ_WIDTH, item.XsSqInfo.Width));
            entries.Add(new HashEntry(KEY_XS_SQ_PATH, item.XsSqInfo.Path));
            entries.Add(new HashEntry(KEY_XS_SQ_SIZE, item.XsSqInfo.Size));
        }

        if(item.SmInfo != null)
        {
            entries.Add(new HashEntry(KEY_SM_HEIGHT, item.SmInfo.Height));
            entries.Add(new HashEntry(KEY_SM_WIDTH, item.SmInfo.Width));
            entries.Add(new HashEntry(KEY_SM_PATH, item.SmInfo.Path));
            entries.Add(new HashEntry(KEY_SM_SIZE, item.SmInfo.Size));
        }

        if(item.MdInfo != null)
        {
            entries.Add(new HashEntry(KEY_MD_HEIGHT, item.MdInfo.Height));
            entries.Add(new HashEntry(KEY_MD_WIDTH, item.MdInfo.Width));
            entries.Add(new HashEntry(KEY_MD_PATH, item.MdInfo.Path));
            entries.Add(new HashEntry(KEY_MD_SIZE, item.MdInfo.Size));
        }

        if(item.LgInfo != null)
        {
            entries.Add(new HashEntry(KEY_LG_HEIGHT, item.LgInfo.Height));
            entries.Add(new HashEntry(KEY_LG_WIDTH, item.LgInfo.Width));
            entries.Add(new HashEntry(KEY_LG_PATH, item.LgInfo.Path));
            entries.Add(new HashEntry(KEY_LG_SIZE, item.LgInfo.Size));
        }

        if(item.PrtInfo != null)
        {
            entries.Add(new HashEntry(KEY_PRT_HEIGHT, item.PrtInfo.Height));
            entries.Add(new HashEntry(KEY_PRT_WIDTH, item.PrtInfo.Width));
            entries.Add(new HashEntry(KEY_PRT_PATH, item.PrtInfo.Path));
            entries.Add(new HashEntry(KEY_PRT_SIZE, item.PrtInfo.Size));
        }

        if(item.SrcInfo != null)
        {
            entries.Add(new HashEntry(KEY_SRC_HEIGHT, item.SrcInfo.Height));
            entries.Add(new HashEntry(KEY_SRC_WIDTH, item.SrcInfo.Width));
            entries.Add(new HashEntry(KEY_SRC_PATH, item.SrcInfo.Path));
            entries.Add(new HashEntry(KEY_SRC_SIZE, item.SrcInfo.Size));
        }

        return entries.ToArray();
    }

    protected override Photo ParseSingleInternal(ReadOnlySpan<RedisValue> values)
    {
        var createDate = values[2] == RedisValue.Null ? (DateTime?)null : DeserializeDate(values[2]!);

        return new Photo
        {
            Id = (int)values[0],
            CategoryId = (short)values[1],
            CreateDate = createDate,
            Latitude = (float?)values[3],
            Longitude = (float?)values[4],
            XsInfo = new MultimediaInfo
            {
                Height = (short)values[5],
                Width = (short)values[6],
                Path = values[7]!,
                Size = (long)values[8]
            },
            XsSqInfo = new MultimediaInfo
            {
                Height = (short)values[9],
                Width = (short)values[10],
                Path = values[11]!,
                Size = (long)values[12]
            },
            SmInfo = new MultimediaInfo
            {
                Height = (short)values[13],
                Width = (short)values[14],
                Path = values[15]!,
                Size = (long)values[16]
            },
            MdInfo = new MultimediaInfo
            {
                Height = (short)values[17],
                Width = (short)values[18],
                Path = values[19]!,
                Size = (long)values[20]
            },
            LgInfo = new MultimediaInfo
            {
                Height = (short)values[21],
                Width = (short)values[22],
                Path = values[23]!,
                Size = (long)values[24]
            },
            PrtInfo = new MultimediaInfo
            {
                Height = (short)values[25],
                Width = (short)values[26],
                Path = values[27]!,
                Size = (long)values[28]
            },
            SrcInfo = new MultimediaInfo
            {
                Height = (short)values[29],
                Width = (short)values[30],
                Path = values[31]!,
                Size = (long)values[32]
            }
        };
    }
}
