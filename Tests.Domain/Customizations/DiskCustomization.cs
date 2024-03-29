using AutoFixture;
using MyDisks.Domain.Disks;

namespace MyDisks.Tests.Domain.Customizations;

public class DiskCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<Name>(composer => composer.FromFactory((string value) => new Name(value.Substring(30))));
        fixture.Customize<Disk>(r => r
            .With(x => x.ReleaseDate, DateTime.Now.AddDays(-1)));
    }
}