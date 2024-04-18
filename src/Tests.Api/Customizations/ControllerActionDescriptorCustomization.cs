using System.Reflection;
using AutoFixture;
using Microsoft.AspNetCore.Mvc.Controllers;
using Moq;

namespace MyDisks.Tests.Api.Customizations;

public class ControllerActionDescriptorCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<ControllerActionDescriptor>(builder => builder.With(b => b.MethodInfo
                , fixture.Create<Mock<MethodInfo>>()
                    .Object)
            .Without(b => b.ControllerTypeInfo));
    }
}