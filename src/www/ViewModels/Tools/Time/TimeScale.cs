namespace MawMvcApp.ViewModels.Tools.Time;

public class TimeScale
{
    public static readonly TimeScale Seconds = new("Seconds", 1);
    public static readonly TimeScale Minutes = new("Minutes", 60);
    public static readonly TimeScale Hours = new("Hours", 60 * 60);
    public static readonly TimeScale Days = new("Days", 60 * 60 * 24);
    public static readonly TimeScale Years = new("Years", 60 * 60 * 24 * 365.242);

    public static readonly TimeScale[] AllScales = new TimeScale[] {
            Seconds,
            Minutes,
            Hours,
            Days,
            Years
        };

    public string Name { get; }
    public double SecondsInUnit { get; }

    public TimeScale(string name, double secondsInUnit)
    {
        Name = name;
        SecondsInUnit = secondsInUnit;
    }
}
