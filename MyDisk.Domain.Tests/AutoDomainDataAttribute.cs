namespace MyDisk.Domain.Tests;

public class AutoDomainDataAttribute : AutoDataAttribute
{
    public AutoDomainDataAttribute() : base(() => new Fixture().Customize(new AutoMoqCustomization()))
    {
    }
}