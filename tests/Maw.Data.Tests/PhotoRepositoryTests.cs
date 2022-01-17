using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Maw.Data.Tests;

public class PhotoRepositoryTests
{
    [Fact]
    public async Task GetAllCategories_ShouldReturnPopulatedObjects()
    {
        var repo = GetRepo();

        var categories = await repo.GetAllCategoriesAsync(new string[] { "friend" }).ConfigureAwait(false);

        Assert.NotNull(categories);
        Assert.NotNull(categories.First().TeaserImage);
        Assert.NotNull(categories.First().TeaserImage.Path);
    }

    [Fact]
    public async Task GetPhotos_ShouldReturnPopulatedObjects()
    {
        var repo = GetRepo();

        var photos = await repo.GetPhotosForCategoryAsync(1, new string[] { "friend" }).ConfigureAwait(false);

        Assert.NotNull(photos);
        Assert.NotNull(photos.First().XsInfo);
        Assert.NotNull(photos.First().XsInfo.Path);
    }

    PhotoRepository GetRepo()
    {
        return new PhotoRepository(GetConnectionString());
    }

    string GetConnectionString()
    {
        var connString = Environment.GetEnvironmentVariable("MAW_API_Environment__DbConnectionString");

        if (string.IsNullOrEmpty(connString))
        {
            throw new ApplicationException("Did not find a valid connection string in the environment variable!");
        }

        return connString;
    }
}
