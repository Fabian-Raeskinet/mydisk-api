namespace MyDisk.Api.Tests.Controllers;

public class GetAllDisksTests
{
    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(6)]
    public void ReturnsBadRequest(int value)
    {
        Assert.True(IsOdd(value));
    }

    private static bool IsOdd(int value)
    {
        return value % 2 == 0;
    }
}