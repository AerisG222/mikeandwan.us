namespace MawMvcApp.ViewModels.Tools.FileSize;

public class FileSizeUnit
{
    public static readonly FileSizeUnit UnitB = new FileSizeUnit("B", 1);
    public static readonly FileSizeUnit UnitKB = new FileSizeUnit("KB", 1024);
    public static readonly FileSizeUnit UnitMB = new FileSizeUnit("MB", 1024 * 1024);
    public static readonly FileSizeUnit UnitGB = new FileSizeUnit("GB", 1024 * 1024 * 1024);

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
