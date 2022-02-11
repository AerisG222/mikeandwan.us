using System;
using Xunit;
using Maw.Domain.Models.Blogs;
using Maw.Domain.Models.Photos;

namespace Maw.Domain.Models.Tests;

public class RecordEqualityTest
{
    [Fact]
    public void FlatRecordEqualityTest()
    {
        var b1 = GetBlog(1);
        var b2 = GetBlog(1);
        var b3 = GetBlog(2);

        Assert.Equal(b1, b1);
        Assert.Equal(b1, b2);
        Assert.NotEqual(b1, b3);
    }

    [Fact]
    public void NestedRecordEqualityTest()
    {
        var c1 = GetCategory(1);
        var c2 = GetCategory(1);
        var c3 = GetCategory(2);

        Assert.Equal(c1, c1);
        Assert.Equal(c1, c2);
        Assert.NotEqual(c1, c3);
    }

    Blog GetBlog(short id)
    {
        return new Blog
        {
            Id = id,
            Title = "title",
            Copyright = "copyright",
            Description = "description",
            LastPostDate = new DateTime(2000, 01, 01)
        };
    }

    Category GetCategory(short id)
    {
        return new Category
        {
            Id = id,
            Name = "name",
            Year = 200,
            CreateDate = new DateTime(2000, 01, 01),
            Latitude = null,
            Longitude = null,
            PhotoCount = 10,
            TotalSizeXs = 20,
            TotalSizeXsSq = 30,
            TotalSizeSm = 40,
            TotalSizeMd = 50,
            TotalSizeLg = 60,
            TotalSizePrt = 70,
            TotalSizeSrc = 80,
            TotalSize = 90,
            TeaserImage = new MultimediaInfo
            {
                Height = 10,
                Width = 20,
                Path = "abc",
                Size = 30
            },
            TeaserImageSq = new MultimediaInfo
            {
                Height = 40,
                Width = 50,
                Path = "def",
                Size = 60
            },
            IsMissingGpsData = true
        };
    }
}
