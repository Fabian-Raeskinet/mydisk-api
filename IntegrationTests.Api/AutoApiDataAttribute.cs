using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using MyDisks.Tests.Domain.Customizations;

namespace MyDisks.IntegrationTests.Api;

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
            new DiskCustomization(),
            new PseudonymCustomization(),
            new ReviewCustomization()
        )
    {
    }
}