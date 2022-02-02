using System;
using Dapper;
using StackExchange.Redis;
using Maw.Cache.Blogs;
using Maw.Cache.Photos;
using Maw.Cache.Videos;
using Maw.Data;

namespace Maw.Cache.Tests;

public static class TestHelper
{
    static TestHelper()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    public static readonly string[] Roles = new string[] { "friend", "demo" };

    public static BlogRepository BlogRepository { get => new BlogRepository(GetDbConnectionString()); }
    public static PhotoRepository PhotoRepository { get => new PhotoRepository(GetDbConnectionString()); }
    public static VideoRepository VideoRepository { get => new VideoRepository(GetDbConnectionString()); }

    static ConnectionMultiplexer Redis = ConnectionMultiplexer.Connect("localhost");

    public static BlogCache BlogCache { get => new BlogCache(Redis.GetDatabase()); }
    public static PhotoCache PhotoCache { get => new PhotoCache(Redis.GetDatabase()); }
    public static VideoCache VideoCache { get => new VideoCache(Redis.GetDatabase()); }

    static string GetDbConnectionString()
    {
        var connString = Environment.GetEnvironmentVariable("MAW_API_Environment__DbConnectionString");

        if (string.IsNullOrEmpty(connString))
        {
            throw new ApplicationException("Did not find a valid connection string in the environment variable!");
        }

        return connString;
    }
}
