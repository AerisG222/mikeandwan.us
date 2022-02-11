namespace Maw.Cache.Sync;

public interface IDelayCalculator
{
    int CalculateRandomizedDelay(int baseDelayMs, float fluctuationPercentage);
}
