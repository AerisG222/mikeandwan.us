namespace MawMvcApp.ViewModels.Tools.Bandwidth;

public class BandwidthSizeResult
{
    public string Name { get; }
    public string Description { get; }
    public double Time { get; }

    public BandwidthSizeResult(string name, string description, double time)
    {
        Name = name;
        Description = description;
        Time = time;
    }
}
