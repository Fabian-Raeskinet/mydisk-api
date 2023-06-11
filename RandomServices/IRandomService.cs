namespace MyDisk.RandomServices;

public interface IRandomService
{
    int GetRandomValue(int minValue, int maxValue);
}