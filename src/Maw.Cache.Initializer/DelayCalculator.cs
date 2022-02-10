namespace Maw.Cache.Initializer;

internal class DelayCalculator
    : IDelayCalculator
{
    public int CalculateRandomizedDelay(int baseDelayMs, float fluctuationPercentage)
    {
        if(fluctuationPercentage <= 0)
        {
            return baseDelayMs;
        }

        var flux = Convert.ToInt32(baseDelayMs * fluctuationPercentage);
        var min = Math.Min(0, baseDelayMs - flux);
        var max = baseDelayMs + flux;
        var rand = new Random();

        return Convert.ToInt32(rand.NextInt64(min, max));
    }
}
