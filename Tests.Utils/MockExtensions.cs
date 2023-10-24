using Moq;

namespace MyDisks.Tests.Utils;

public static class MockExtensions
{
    public static Mock<T> AsMock<T>(this T t) where T : class
    {
        return Mock.Get(t);
    }
}