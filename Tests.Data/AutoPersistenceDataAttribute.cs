using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using MyDisks.Tests.Domain.Customizations;

namespace Tests.Data;

public class AutoPersistenceDataAttribute : AutoDataAttribute
{
    public AutoPersistenceDataAttribute() : base(() => new Fixture().Customize(new PersistenceCustomization()))
    {
    }
}

public class InlineAutoPersistenceDataAttribute : InlineAutoDataAttribute
{
    public InlineAutoPersistenceDataAttribute(params object[] values) : base(new AutoPersistenceDataAttribute(), values)
    {
    }
}

internal class PersistenceCustomization : CompositeCustomization
{
    public PersistenceCustomization()
        : base(
            new AutoMoqCustomization(),
            new DiskCustomization(),
            new PseudonymCustomization(),
            new ReviewCustomization())
    {
    }
}