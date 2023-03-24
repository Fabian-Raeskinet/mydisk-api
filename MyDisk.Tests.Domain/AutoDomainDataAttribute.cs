using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace MyDisk.Tests.Domain;

public class AutoDomainDataAttribute : AutoDataAttribute
{
    public AutoDomainDataAttribute() : base(() => new Fixture().Customize(new DomainCustomization()))
    {
    }
}

public class InlineAutoDomainDataAttribute : InlineAutoDataAttribute
{
    public InlineAutoDomainDataAttribute(params object[] values) : base(new AutoDomainDataAttribute(), values)
    {
    }
}

internal class DomainCustomization : CompositeCustomization
{
    public DomainCustomization()
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