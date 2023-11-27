using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;

namespace MyDisks.Tests.Api.Customizations;

public class ActionContextCustomization : ICustomization
{
    public void Customize(IFixture fixture) =>
        fixture.Customize<ActionContext>(c =>
            c.With(x => x.HttpContext, new DefaultHttpContext())
                .With(x => x.RouteData, new RouteData())
                .With(x => x.ActionDescriptor, new ActionDescriptor())
        );
}