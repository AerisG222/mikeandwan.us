namespace Maw.Domain;

public class EnvironmentConfig
{
    public string Name { get; set; } = null!;
    public string AssetsPath { get; set; } = null!;
    public string DbConnectionString { get; set; } = null!;
    public string RedisConnectionString { get; set; } = null!;
}
