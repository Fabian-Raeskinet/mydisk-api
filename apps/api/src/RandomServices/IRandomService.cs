namespace MyDisks.RandomServices;

public interface IRandomService
{
    int GetRandomValue(int minValue, int maxValue);
}