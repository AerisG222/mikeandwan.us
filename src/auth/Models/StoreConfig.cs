namespace MawAuth.Models;

public class StoreConfig
{
    public string ConnectionString { get; }

    public StoreConfig(string connString)
    {
        ConnectionString = connString ?? throw new ArgumentNullException(nameof(connString));
    }
}
