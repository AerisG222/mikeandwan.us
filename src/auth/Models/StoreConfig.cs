namespace MawAuth.Models;

public class StoreConfig
{
    public string ConnectionString { get; }

    public StoreConfig(string connString)
    {
        ArgumentNullException.ThrowIfNull(connString);

        ConnectionString = connString;
    }
}
