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

    public static BlogRepository BlogRepository => new(GetDbConnectionString());
    public static PhotoRepository PhotoRepository => new(GetDbConnectionString());
    public static VideoRepository VideoRepository => new(GetDbConnectionString());

    static readonly ConnectionMultiplexer Redis = ConnectionMultiplexer.Connect("localhost");

    public static BlogCache BlogCache => new(Redis.GetDatabase());
    public static PhotoCache PhotoCache => new(Redis.GetDatabase());
    public static VideoCache VideoCache => new(Redis.GetDatabase());

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
