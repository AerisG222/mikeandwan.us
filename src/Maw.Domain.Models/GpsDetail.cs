namespace Maw.Domain.Models;

public class GpsDetail
{
    public GpsCoordinate Source { get; set; } = null!;
    public GpsCoordinate Override { get; set; } = null!;
}
