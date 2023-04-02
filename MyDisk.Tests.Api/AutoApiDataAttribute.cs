using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using MyDisk.Tests.Api.Customizations;

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
        : base(
            new AutoMoqCustomization(),
            new ActionContextCustomization(),
            new ExceptionContextCustomization()
        )
    {
    }
}