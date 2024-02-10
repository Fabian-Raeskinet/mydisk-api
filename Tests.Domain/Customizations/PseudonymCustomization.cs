using AutoFixture;
using MyDisks.Domain.Authors;
using MyDisks.Domain.Disks;

namespace MyDisks.Tests.Domain.Customizations;

public class PseudonymCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<Pseudonym>(composer => composer.FromFactory((string value) => new Pseudonym(value.Substring(30))));
    }
}