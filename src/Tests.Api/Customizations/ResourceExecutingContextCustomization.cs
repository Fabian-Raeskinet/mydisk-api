using AutoFixture;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MyDisks.Tests.Api.Customizations;

public class ResourceExecutingContextCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<ResourceExecutingContext>(builder => builder
            .With(b => b.ActionDescriptor
                , fixture.Create<ControllerActionDescriptor>())
            .Without(b => b.Result));
    }
}