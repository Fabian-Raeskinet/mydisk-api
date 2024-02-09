using System.Reflection;
using AutoFixture;
using Moq;

namespace MyDisks.Tests.Api.Customizations;

public class MethodInfoCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<Mock<MethodInfo>>(builder => builder.Do(m => m.Setup(mi => mi.CustomAttributes)
            .Returns(Array.Empty<CustomAttributeData>)));
    }
}