using System.Reflection;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;

namespace MyDisks.Tests.Api.Customizations;

public class BindingInfoCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        // fix error on controller creation
        fixture.Customize<BindingInfo>(c => c.With(bi => bi.BinderType
            , fixture.Create<IModelBinder>()
                .GetType()));


        fixture.Customize<Mock<HttpContext>>(builder => builder.Do(c => c.Setup(m => m.RequestServices)
            .Returns(() => fixture
                .Create<Mock<IServiceProvider>>()
                .Object)));
        fixture.Customize<Mock<MethodInfo>>(builder => builder.Do(m => m.Setup(mi => mi.CustomAttributes)
            .Returns(Array.Empty<CustomAttributeData>)));
        fixture.Customize<ControllerActionDescriptor>(builder => builder.With(b => b.MethodInfo
                , fixture.Create<Mock<MethodInfo>>()
                    .Object)
            .Without(b => b.ControllerTypeInfo));
        fixture.Customize<ResourceExecutingContext>(builder => builder
            .With(b => b.ActionDescriptor
                , fixture.Create<ControllerActionDescriptor>())
            .Without(b => b.Result));
    }
}