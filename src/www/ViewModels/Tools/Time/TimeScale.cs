namespace MawMvcApp.ViewModels.Tools.Time;

public class TimeScale
{
    public static readonly TimeScale Seconds = new TimeScale("Seconds", 1);
    public static readonly TimeScale Minutes = new TimeScale("Minutes", 60);
    public static readonly TimeScale Hours = new TimeScale("Hours", 60 * 60);
    public static readonly TimeScale Days = new TimeScale("Days", 60 * 60 * 24);
    public static readonly TimeScale Years = new TimeScale("Years", 60 * 60 * 24 * 365.242);

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
