using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace MyDisk.Tests.Api;

public class AutoApiDataAttribute : AutoDataAttribute
{
    public AutoApiDataAttribute() : base(() => new Fixture().Customize(new AutoMoqCustomization()))
    {
    }
}