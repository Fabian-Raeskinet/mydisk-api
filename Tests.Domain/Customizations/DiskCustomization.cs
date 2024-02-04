using AutoFixture;
using MyDisks.Domain.Disks;

namespace MyDisks.Tests.Domain.Customizations;

public class DiskCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<Disk>(composer => composer.With(x => x.Name, fixture.Create<string>().Substring((29))));
    }
}