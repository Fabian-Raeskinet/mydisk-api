using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace MyDisk.Tests.Api;

public class AutoApiDataAttribute : AutoDataAttribute
{
    public AutoApiDataAttribute() : base(() => new Fixture().Customize(new ApiCustomization()))
    {
    }
}

public class InlineAutoApiDataAttribute : InlineAutoDataAttribute
{
    public InlineAutoApiDataAttribute(params object[] values) : base(new AutoApiDataAttribute(), values)
    {
    }
}

internal class ApiCustomization : CompositeCustomization
{
    public ApiCustomization()
        : base(new CommonCustomization(), new AutoMoqCustomization())
    {
    }
}

internal class CommonCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
    }
}