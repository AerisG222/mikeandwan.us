namespace MawMvcApp.ViewModels.Tools.FileSize;

public class Result
{
    public string Name { get; }
    public double SizeInUnits { get; }

    public Result(string name, double sizeInUnits)
    {
        Name = name;
        SizeInUnits = sizeInUnits;
    }
}
