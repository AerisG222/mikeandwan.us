namespace MawMvcApp.ViewModels.Tools.FileSize;

public class FileSizeUnit
{
    public static readonly FileSizeUnit UnitB = new("B", 1);
    public static readonly FileSizeUnit UnitKB = new("KB", 1024);
    public static readonly FileSizeUnit UnitMB = new("MB", 1024 * 1024);
    public static readonly FileSizeUnit UnitGB = new("GB", 1024 * 1024 * 1024);

    public static readonly FileSizeUnit[] AllUnits = new FileSizeUnit[] {
            UnitB,
            UnitKB,
            UnitMB,
            UnitGB
        };

    public string Name { get; }
    public long BytesInUnit { get; }

    public FileSizeUnit(string name, long bytesInUnit)
    {
        Name = name;
        BytesInUnit = bytesInUnit;
    }
}
