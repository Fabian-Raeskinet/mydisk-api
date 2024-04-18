using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using MyDisks.Tests.Domain.Customizations;
using MyDisks.Tests.Services.Customizations;

namespace MyDisks.Tests.Services;

public class AutoServiceDataAttribute : AutoDataAttribute
{
    public AutoServiceDataAttribute() : base(() => new Fixture().Customize(new ServiceCustomization()))
    {
    }
}

public class InlineAutoServiceDataAttribute : InlineAutoDataAttribute
{
    public InlineAutoServiceDataAttribute(params object?[]? values) : base(new AutoServiceDataAttribute(), values)
    {
    }
}

internal class ServiceCustomization : CompositeCustomization
{
    public ServiceCustomization()
        : base(
            new AutoMoqCustomization(),
            new DiskCustomization(),
            new PseudonymCustomization(),
            new ReviewCustomization(),
            new CreateReviewCommandRequestCustomization())
    {
    }
}