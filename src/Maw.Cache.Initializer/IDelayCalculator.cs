namespace Maw.Cache.Initializer;

public interface IDelayCalculator
{
    int CalculateRandomizedDelay(int baseDelayMs, float fluctuationPercentage);
}
