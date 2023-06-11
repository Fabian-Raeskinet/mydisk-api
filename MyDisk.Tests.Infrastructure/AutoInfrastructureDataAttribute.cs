using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace MyDisk.Tests.Infrastructure;

public class AutoInfrastructureDataAttribute : AutoDataAttribute
{
    public AutoInfrastructureDataAttribute() : base(() => new Fixture().Customize(new ServiceCustomization()))
    {
    }
}

public class InlineAutoInfrastructureDataAttribute : InlineAutoDataAttribute
{
    public InlineAutoInfrastructureDataAttribute(params object?[]? values) : base(new AutoInfrastructureDataAttribute(), values)
    {
    }
}

internal class ServiceCustomization : CompositeCustomization
{
    public ServiceCustomization()
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