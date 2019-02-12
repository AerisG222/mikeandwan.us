namespace MawMvcApp.ViewModels.Tools.Time
{
    public class Result {
        public double LengthOfTime { get; }
        public string TimeUnit { get; }


        public Result(string timeUnit, double lengthOfTime)
        {
            LengthOfTime = lengthOfTime;
            TimeUnit = timeUnit;
        }
    }
}
