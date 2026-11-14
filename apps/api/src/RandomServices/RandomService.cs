namespace MyDisks.RandomServices;

public class RandomService : IRandomService
{
    public RandomService()
    {
        Random = new Random();
    }

    private Random Random { get; }

    public int GetRandomValue(int minValue, int maxValue)
    {
        return Random.Next(minValue, maxValue);
    }
}