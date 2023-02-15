using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace MyDisk.Tests.Services;

public class AutoServiceDataAttribute : AutoDataAttribute
{
    public AutoServiceDataAttribute() : base(() => new Fixture().Customize(new AutoMoqCustomization()))
    {
    }
}